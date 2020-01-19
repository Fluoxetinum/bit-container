using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Storage;
using BitContainer.DataAccess;
using BitContainer.DataAccess.DataProviders;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Store;
using BitContainer.Shared;
using BitContainer.Shared.Auth;
using BitContainer.Shared.StreamHelpers;
using BitContainer.StorageService.Helpers;
using BitContainer.StorageService.Managers.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BitContainer.StorageService.Managers
{
    public class CLoadsManager : ILoadsManager
    {
        private readonly ISqlDbHelper _dbHelper;
        private readonly ILogger<CLoadsManager> _logger;
        private readonly IStorageProvider _storageProvider;

        private readonly TcpListener _uploadToServerListener;
        private readonly TcpListener _downloadFromServerListener;

        private Task _uploadToServerTask;
        private Boolean _uploadToServerTaskFlag;
        private Task _downloadFromServerTask;
        private Boolean _downloadFromServerTaskFlag;

        public CTransmissionEndPointContract EndPointToUploadToServer { get; private set; }
        public CTransmissionEndPointContract EndPointToDownloadFromServer { get; private set; }

        public CLoadsManager(ISqlDbHelper dbHelper, IStorageProvider storageProvider, ILogger<CLoadsManager> logger)
        {
            _dbHelper = dbHelper;
            _storageProvider = storageProvider;
            _logger = logger;

            _uploadToServerListener = new TcpListener(IPAddress.Loopback, 0);
            _downloadFromServerListener = new TcpListener(IPAddress.Loopback, 0);

            _downloadFromServerTaskFlag = true;
            _downloadFromServerTask = Task.Run(StartDownloadFromServer);

            _uploadToServerTaskFlag = true;
            _uploadToServerTask = Task.Run(StartUploadToServerListener);
        }

        private Boolean CheckUserRights(String jwtString, Guid entityId, ERestrictedAccessType accessType, out Guid userId)
        {
            userId = Guid.Empty;
            var jwtHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validParams = AuthOptions.GetTokenValidationParameters();
            try
            {
                ClaimsPrincipal principal = jwtHandler.ValidateToken(jwtString, validParams, out _);
                userId = new Guid(principal.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
                ERestrictedAccessType userAccess = _storageProvider.Shares.CheckStorageEntityAccess(entityId, userId);
                return userAccess >= accessType;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }

        private async Task StartDownloadFromServer()
        {
            try
            {
                _downloadFromServerListener.Start();

                IPEndPoint downEndPoint = (IPEndPoint)_downloadFromServerListener.LocalEndpoint;
                EndPointToDownloadFromServer = CTransmissionEndPointContract.Create(downEndPoint.Address, (short)downEndPoint.Port);

                while (_downloadFromServerTaskFlag)
                {
                    using TcpClient client = await _downloadFromServerListener.AcceptTcpClientAsync();
                    try
                    {
                        await ProcessDownloadFromServerClient(client);
                    }
                    catch (Exception e)
                    {
                        IPEndPoint clientAddress = (IPEndPoint) client.Client.RemoteEndPoint;
                        _logger.LogError(e, $"Error while download, client - {clientAddress}");
                    }
                }
            }
            catch (SocketException e)
            {
                _logger.LogError(e, "Error upon creating download sockets.");
            }
            finally
            {
                _downloadFromServerListener.Stop();
            }
        }

        private async Task ProcessDownloadFromServerClient(TcpClient client)
        {
            await using NetworkStream networkStream = client.GetStream();

            Guid id = await networkStream.ReadGuidAsync();
            String jwt = await networkStream.ReadStringAsync();

            if (!CheckUserRights(jwt, id, ERestrictedAccessType.Read, out _)) return;

            IStorageEntity entity = _storageProvider.StorageEntities.GetStorageEntity(id);

            switch (entity)
            {
                case CFile file: await PassFileToClient(networkStream, file);
                    break;
                case CDirectory dir: await PassDirArchiveToClient(networkStream, dir);
                    break;
                default:
                    throw new InvalidCastException($"Unexpected type of '{nameof(entity)}'");
            }
        }

        private async Task PassFileToClient(NetworkStream clientStream, CFile file)
        {
            await _dbHelper.ExecuteTransactionAsync(async (command) =>
            {
                var query = new GetFileStreamPathQuery(file.Id);
                CFileStreamInfo fileStreamInfo = query.Execute(command);
                await using var fileStream = 
                    new SqlFileStream(fileStreamInfo.Path, fileStreamInfo.TransactionContext, FileAccess.Read);
                
                await StreamEx.WriteToStream(fromStream:fileStream, toStream:clientStream, file.Size);
            });
        }

        private async Task PassDirArchiveToClient(NetworkStream networkStream, CDirectory dirToPass)
        {
            Dictionary<Int32, List<IStorageEntity>> children = 
                _storageProvider.StorageEntities.GetAllChildren(dirToPass.Id);

            MemoryStream memory = new MemoryStream();
            using (ZipArchive archive = new ZipArchive(memory, ZipArchiveMode.Create))
            {
                Int32 level = 1;

                IStorageEntity rootDir = children[level].Single();
                Dictionary<Guid, String> paths = new Dictionary<Guid, String>();
                paths[rootDir.Id] = $@"{rootDir.Name}\";
                archive.CreateEntry(paths[rootDir.Id]);

                level++;

                while (children.ContainsKey(level))
                {
                    List<IStorageEntity> entity = children[level];

                    foreach (var storageEntity in entity)
                    {
                        switch (storageEntity)
                        {
                            case CFile file:
                                byte[] fileData = _storageProvider.StorageEntities.GetAllFileData(file.Id);
                                ZipArchiveEntry entry = archive.CreateEntry($@"{paths[file.ParentId]}{file.Name}");

                                using (var stream = entry.Open())
                                {
                                    await stream.WriteAsync(fileData, 0, fileData.Length);
                                }

                                break;
                            case CDirectory dir:
                                String path = $@"{paths[dir.ParentId]}{dir.Name}\";
                                archive.CreateEntry(path);
                                paths[dir.Id] = path;
                                break;
                            default:
                                throw new InvalidCastException($"Unexpected type of '{nameof(storageEntity)}'");
                        }
                    }
                    level++;
                }
            }

            Byte[] archiveBytes = memory.ToArray();
            await networkStream.WriteAsync(archiveBytes, 0, archiveBytes.Length);
        }
        
        private async Task StartUploadToServerListener()
        {
            try
            {
                _uploadToServerListener.Start();

                IPEndPoint upEndPoint = (IPEndPoint)_uploadToServerListener.LocalEndpoint;
                EndPointToUploadToServer = CTransmissionEndPointContract.Create(upEndPoint.Address, (short)upEndPoint.Port);

                while (_uploadToServerTaskFlag)
                {
                    using TcpClient client = await _uploadToServerListener.AcceptTcpClientAsync();
                    try
                    {
                        await ProccessUploadToServerClient(client);
                    }
                    catch (Exception e)
                    {
                        IPEndPoint clientAddress = (IPEndPoint) client.Client.RemoteEndPoint;
                        _logger.LogError(e, $"Error while upload, client - {clientAddress}");
                    }
                }
            }
            catch (SocketException e)
            {
                _logger.LogError(e, "Error upon creating upload sockets.");
            }
            finally
            {
                _downloadFromServerListener.Stop();
            }
            
        }

        private async Task ProccessUploadToServerClient(TcpClient client)
        {
            await using NetworkStream networkStream = client.GetStream();

            String fileName = await networkStream.ReadStringAsync();
            Guid parentId = await networkStream.ReadGuidAsync();

            String jwt = await networkStream.ReadStringAsync();
            if (!CheckUserRights(jwt, parentId, ERestrictedAccessType.Write, out var ownerId)) return;

            if (!parentId.IsRootDir()) ownerId = _storageProvider.Shares.GetStorageEntityOwner(parentId);

            Int64 fileSize = await networkStream.ReadInt64Async();

            CFile newFile = await TakeFileFromClient(networkStream, parentId, ownerId, fileName, fileSize);

            await networkStream.WriteGuidAsync(newFile.Id);
        }

        private async Task<CFile> TakeFileFromClient(NetworkStream clientStream, Guid parentId, Guid ownerId, String name, Int64 size)
        {
            CFile file = null;
            await _dbHelper.ExecuteTransactionAsync(async (command) =>
            {
                file = _storageProvider.StorageEntities.AddEmptyFile(parentId, ownerId, name, command);

                var query = new GetFileStreamPathQuery(file.Id);
                CFileStreamInfo fileStreamInfo = query.Execute(command);
                await using var fileStream = 
                    new SqlFileStream(fileStreamInfo.Path, fileStreamInfo.TransactionContext, FileAccess.Write);
                
                await StreamEx.WriteToStream(fromStream:clientStream, toStream:fileStream, size);
            });
            return file;
        }
    }
}
