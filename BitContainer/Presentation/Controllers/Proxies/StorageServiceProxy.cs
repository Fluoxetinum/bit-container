using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Controllers.Proxies.Requests;
using BitContainer.Presentation.Models;
using BitContainer.Shared.Http;
using BitContainer.Shared.Http.Requests;
using Newtonsoft.Json;

namespace BitContainer.Presentation.Controllers.Proxies
{
    public class StorageServiceProxy
    {
        private static readonly String _serviceUrl = ConfigurationManager.AppSettings["ServiceUrl"];
        private static readonly String _storageControllerName = "storage";

        private static readonly String _getOwnerStorageEntitiesRequest = $@"{_serviceUrl}/{_storageControllerName}/entities/";
        private static readonly String _getSharedStorageEntitiesRequest = $@"{_serviceUrl}/{_storageControllerName}/shared_entities/";

        private static readonly String _getUploadEndpointRequest = $@"{_serviceUrl}/{_storageControllerName}/upload_endpoint";
        private static readonly String _getDownloadEndpointRequest = $@"{_serviceUrl}/{_storageControllerName}/download_endpoint";

        private static readonly String _dirRequest = $@"{_serviceUrl}/{_storageControllerName}/dir/";
        private static readonly String _fileRequest = $@"{_serviceUrl}/{_storageControllerName}/file/";
        private static readonly String _dirCopyRequest = $@"{_dirRequest}copy/";
        private static readonly String _fileCopyRequest = $@"{_fileRequest}copy/";
        public static readonly String _shareRequest = $@"{_serviceUrl}/{_storageControllerName}/share/";

        public static readonly String _searchRequest = $@"{_serviceUrl}/{_storageControllerName}/entities/search/";
        public static readonly String _searchSharedRequest = $@"{_serviceUrl}/{_storageControllerName}/shared_entities/search/";

        public static async Task UpdateShare(CNewShareContract newShare)
        {
            var builder = new PutRequestBuilder<CNewShareContract>(_shareRequest, newShare);

            await HttpHelper.Request(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });
        }

        public static async Task<List<CSearchResultContract>> SearchStorageEntities(Guid parentId, String pattern)
        {
            var builder = new GetRequestBuilder(_searchRequest + $"{parentId}/{pattern}");

            return await HttpHelper.Request<List<CSearchResultContract>>(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });
        }
        
        public static async Task<List<CSearchResultContract>> SearchSharedStorageEntities(Guid parentId, string pattern)
        {
            var builder = new GetRequestBuilder(_searchSharedRequest + $"{parentId}/{pattern}");

            return await HttpHelper.Request<List<CSearchResultContract>>(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });
        }


        public static async Task<COwnStorageEntitiesListContract> GetOwnerStorageEntites(Guid parentId)
        {
            var builder = new GetRequestBuilder(_getOwnerStorageEntitiesRequest + parentId);

            return await HttpHelper.Request<COwnStorageEntitiesListContract>(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });
        }

        public static async Task<CRestrictedStorageEntitiesListContract> GetSharedStorageEntites(Guid parentId)
        {
            var builder = new GetRequestBuilder(_getSharedStorageEntitiesRequest + parentId);

            return await HttpHelper.Request<CRestrictedStorageEntitiesListContract>(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });
        }

        public static async Task<IAccessWrapperContract> CreateDirectory(CDirectoryContract dirContract)
        {
            var builder = new PostRequestBuilder<CDirectoryContract>(_dirRequest, dirContract);

            return await HttpHelper.Request<IAccessWrapperContract>(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });
        }

        public static async Task DeleteDirectory(CDirectoryContract dir)
        {
            var builder = new DeleteRequestBuilder<CDirectoryContract>(_dirRequest, dir);

            await HttpHelper.Request(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });
        }

        public static async Task DeleteFile(CFileContract file)
        {
            var builder = new DeleteRequestBuilder<CFileContract>(_fileRequest, file);

            await HttpHelper.Request(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });
        }

        public static async Task RenameDirectory(CDirectoryContract dir)
        {
            var builder = new PutRequestBuilder<CDirectoryContract>(_dirRequest, dir);

            await HttpHelper.Request(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });

        }

        public static async Task RenameFile(CFileContract file)
        {
            var builder = new PutRequestBuilder<CFileContract>(_fileRequest, file);

            await HttpHelper.Request(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });

        }

        public static async Task CopyDirectory(CDirectoryContract dir)
        {
            var builder = new PostRequestBuilder<CDirectoryContract>(_dirCopyRequest, dir);

            await HttpHelper.Request(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });

        }

        public static async Task CopyFile(CFileContract file)
        {
            var builder = new PostRequestBuilder<CFileContract>(_fileCopyRequest, file);

            await HttpHelper.Request(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });

        }

        public static async Task<IAccessWrapperContract> GetFile(Guid id)
        {
            var builder = new GetRequestBuilder(_fileRequest + $"{id}");

            return await HttpHelper.Request<IAccessWrapperContract>(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });

        }
        
        private static readonly int GuidSize = Guid.Empty.ToString().Length;

        public static async Task<IAccessWrapperContract> UploadFile(FileInfo info, Guid parentId,
            IProgress<Double> onProgressPercentChanged, Int32 blockSize = 10000)
        {
            Guid ownerId = AuthController.AuthenticatedUserUiModel.Id;

            CTransmissionEndPointContract endPoint = await GetEndpoint(_getUploadEndpointRequest);

            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(endPoint.Address, endPoint.Port);

                using (NetworkStream networkStream = client.GetStream())
                {
                    byte[] fileName = Encoding.UTF8.GetBytes(info.Name);
                    byte[] fileNameSize = BitConverter.GetBytes(fileName.Length);

                    await networkStream.WriteAsync(fileNameSize, 0, fileNameSize.Length);
                    await networkStream.WriteAsync(fileName, 0, fileName.Length);

                    byte[] parentIdBytes = Encoding.UTF8.GetBytes(parentId.ToString());
                    await networkStream.WriteAsync(parentIdBytes, 0, parentIdBytes.Length);

                    byte[] ownerIdBytes = Encoding.UTF8.GetBytes(ownerId.ToString());
                    await networkStream.WriteAsync(ownerIdBytes, 0, ownerIdBytes.Length);

                    Int32 size = (Int32) info.Length;
                    byte[] fileSize = BitConverter.GetBytes(size);
                    await networkStream.WriteAsync(fileSize, 0, fileSize.Length);

                    using (FileStream reader = File.OpenRead(info.FullName))
                    {
                        Int32 writeBytes = 0;
                        byte[] block = new byte[blockSize];
                        int chunkSize = 1;
                        while (chunkSize > 0)
                        {
                            chunkSize = await reader.ReadAsync(block, 0, block.Length);
                            await networkStream.WriteAsync(block, 0, chunkSize);

                            writeBytes += chunkSize;
                            Double progress = (Double)writeBytes / size * 100;
                            onProgressPercentChanged.Report(progress);
                        }
                    }

                    // <Delivery confirmation> 
                    byte[] idBytes = new byte[GuidSize];
                    await networkStream.ReadAsync(idBytes, 0, idBytes.Length);
                    Guid id = new Guid(Encoding.UTF8.GetString(idBytes));
                    // </Delivery confirmation> 

                    IAccessWrapperContract newFile = await GetFile(id);

                    return newFile;
                }
            }
        }

        public static async Task LoadEntity(String path, IStorageEntityUiModel entity, 
            IProgress<Double> onProgressPercentChanged, Int32 blockSize = 10000)
        {
            Guid ownerId = AuthController.AuthenticatedUserUiModel.Id;

            EEntityTypeContract typeContract;
            Int64 size; 

            if (entity is CFileUiModel file)
            {
                typeContract = EEntityTypeContract.File;
                size = (Int32)file.Size; // TODO: Big files
            }
            else if (entity is CDirectoryUiModel dir)
            {
                size = 1;// TODO: Dir sizes
                typeContract = EEntityTypeContract.Directory;
                path += ".zip";
            }
            else
            {
                throw new NotSupportedException("not supported entity type.");
            }
            
            CTransmissionEndPointContract endPoint = await GetEndpoint(_getDownloadEndpointRequest);
            
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(endPoint.Address, endPoint.Port);

                using (NetworkStream networkStream = client.GetStream())
                {
                    byte[] typeBytes = BitConverter.GetBytes((Int32)typeContract);
                    await networkStream.WriteAsync(typeBytes, 0, typeBytes.Length);

                    byte[] idBytes = Encoding.UTF8.GetBytes(entity.Id.ToString());
                    await networkStream.WriteAsync(idBytes, 0, idBytes.Length);

                    byte[] ownerIdBytes = Encoding.UTF8.GetBytes(ownerId.ToString());
                    await networkStream.WriteAsync(ownerIdBytes, 0, ownerIdBytes.Length);

                    using (FileStream writer = File.OpenWrite(path))
                    {
                        Int32 readBytes = 0;
                        byte[] block = new byte[blockSize];
                        int chunkSize = 1;

                        while (chunkSize > 0)
                        {
                            chunkSize = await networkStream.ReadAsync(block, 0, block.Length);
                            await writer.WriteAsync(block, 0, chunkSize);

                            readBytes += chunkSize;
                            Double progress = (Double)readBytes / size * 100;
                            onProgressPercentChanged.Report(progress);
                        }
                    }
                }
            }
        }
        
        private static async Task<CTransmissionEndPointContract> GetEndpoint(String request)
        {
            var builder = new GetRequestBuilder(request);

            return await HttpHelper.Request<CTransmissionEndPointContract>(builder, response =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            });
        }
    }
}
