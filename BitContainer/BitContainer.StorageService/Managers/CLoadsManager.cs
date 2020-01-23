using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.DataAccess;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.StorageEntites;
using BitContainer.Service.Storage.Managers.Interfaces;
using BitContainer.Services.Shared;
using BitContainer.Shared.Models;
using BitContainer.Shared.StreamHelpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Management.Common;

namespace BitContainer.Service.Storage.Managers
{
    public class CLoadsManager : ILoadsManager
    {
        private readonly ISqlDbHelper _dbHelper;
        private readonly ISignalsManager _signalsManager;
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

        public CLoadsManager(ISqlDbHelper dbHelper, 
            IStorageProvider storageProvider, 
            ILogger<CLoadsManager> logger, 
            ISignalsManager signalsManager)
        {
            _dbHelper = dbHelper;
            _storageProvider = storageProvider;
            _logger = logger;
            _signalsManager = signalsManager;

            _uploadToServerListener = new TcpListener(IPAddress.Loopback, 0);
            _downloadFromServerListener = new TcpListener(IPAddress.Loopback, 0);

            _downloadFromServerTaskFlag = true;
            _downloadFromServerTask = Task.Run(StartDownloadFromServer);

            _uploadToServerTaskFlag = true;
            _uploadToServerTask = Task.Run(StartUploadToServerListener);
        }

        private Boolean CheckToken(String jwtString, out CUserId userId)
        {
            userId = new CUserId();
            var jwtHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validParams = AuthOptions.GetTokenValidationParameters();
            try
            {
                ClaimsPrincipal principal = jwtHandler.ValidateToken(jwtString, validParams, out _);
                Guid guidId = new Guid(principal.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
                userId = new CUserId(guidId);
            }
            catch (SecurityTokenException)
            {
                return false;
            }

            return true;
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

            CStorageEntityId id = new CStorageEntityId(await networkStream.ReadGuidAsync());
            String jwt = await networkStream.ReadStringAsync();
            CUserId userId;
            if (!CheckToken(jwt, out userId)) return;

            try
            {
                _storageProvider.Validator.EntityExists(id).HasReadAccess(userId);
            }
            catch (InvalidArgumentException)
            {
                return;
            }
            catch (AuthenticationException)
            {
                return;
            }

            IStorageEntity entity = _storageProvider.Entities.GetStorageEntity(id);

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
                await using var fileStream = _storageProvider.Entities.GetFileStream(command, file.Id, FileAccess.Read);
                await StreamEx.WriteToStream(fromStream:fileStream, toStream:clientStream, file.Size);
            });
        }

        private async Task PassDirArchiveToClient(NetworkStream networkStream, CDirectory dirToPass)
        {
            SortedDictionary<Int32, List<IStorageEntity>> children = 
                _storageProvider.Entities.GetChildrenAsc(dirToPass.Id);

            using ZipArchive archive = new ZipArchive(networkStream, ZipArchiveMode.Create);

            Dictionary<CStorageEntityId, String> paths = new Dictionary<CStorageEntityId, String>();
            paths[dirToPass.Id] = $@"{dirToPass.Name}\";
            archive.CreateEntry(paths[dirToPass.Id]);

            foreach (var level in children)
            {
                foreach (var storageEntity in level.Value)
                {
                    switch (storageEntity)
                    {
                        case CFile file:

                            await _dbHelper.ExecuteTransactionAsync(async (command) =>
                            {
                                await using var fileStream = _storageProvider.Entities.GetFileStream(command, file.Id, FileAccess.Read);

                                ZipArchiveEntry entry = archive.CreateEntry($@"{paths[file.ParentId]}{file.Name}");
                                await StreamEx.WriteToStream(fromStream:fileStream, toStream:entry.Open(), file.Size);
                            });

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
            }
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
            CStorageEntityId parentId = new CStorageEntityId(await networkStream.ReadGuidAsync());
            String jwt = await networkStream.ReadStringAsync();

            CUserId userId;
            if (!CheckToken(jwt, out userId)) return;
            IStorageEntity parent;
            try
            {
                parent =
                _storageProvider.Validator.EntityExists(parentId).HasReadAccess(userId).ToEntity();
                _storageProvider.Validator.EntityNotExists(parentId, userId, fileName);
            }
            catch (InvalidArgumentException)
            {
                return;
            }
            catch (AuthenticationException)
            {
                return;
            }

            Int64 fileSize = await networkStream.ReadInt64Async();

            CSharableEntity newFile = await _storageProvider.Entities.AddFileAsync(networkStream, parent, userId, fileName, fileSize);
            
            _signalsManager.SignalEntityCreation(newFile.AccessWrapper.Entity.Id, userId);
            _signalsManager.SignalStatsUpdate(userId);

            await networkStream.WriteGuidAsync(newFile.AccessWrapper.Entity.Id.ToGuid());
        }

    }
}
