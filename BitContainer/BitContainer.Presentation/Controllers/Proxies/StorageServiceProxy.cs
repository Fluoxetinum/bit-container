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
using BitContainer.Presentation.Models;
using BitContainer.Shared.Http;
using BitContainer.Shared.Http.Requests;
using BitContainer.Shared.StreamHelpers;
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


        public static async Task<List<CAccessWrapperContract>> GetOwnerStorageEntites(Guid parentId)
        {
            var builder = new GetRequestBuilder(_getOwnerStorageEntitiesRequest + parentId);

            return await HttpHelper.Request<List<CAccessWrapperContract>>(builder, response =>
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

        public static async Task<List<CAccessWrapperContract>> GetSharedStorageEntites(Guid parentId)
        {
            var builder = new GetRequestBuilder(_getSharedStorageEntitiesRequest + parentId);

            return await HttpHelper.Request<List<CAccessWrapperContract>>(builder, response =>
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

        public static async Task<CAccessWrapperContract> CreateDirectory(CStorageEntityContract dirContract)
        {
            var builder = new PostRequestBuilder<CStorageEntityContract>(_dirRequest, dirContract);

            return await HttpHelper.Request<CAccessWrapperContract>(builder, response =>
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

        public static async Task DeleteDirectory(CStorageEntityContract dir)
        {
            var builder = new DeleteRequestBuilder<CStorageEntityContract>(_dirRequest, dir);

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

        public static async Task DeleteFile(CStorageEntityContract file)
        {
            var builder = new DeleteRequestBuilder<CStorageEntityContract>(_fileRequest, file);

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

        public static async Task RenameDirectory(CStorageEntityContract dir)
        {
            var builder = new PutRequestBuilder<CStorageEntityContract>(_dirRequest, dir);

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

        public static async Task RenameFile(CStorageEntityContract file)
        {
            var builder = new PutRequestBuilder<CStorageEntityContract>(_fileRequest, file);

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

        public static async Task CopyDirectory(CStorageEntityContract dir)
        {
            var builder = new PostRequestBuilder<CStorageEntityContract>(_dirCopyRequest, dir);

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

        public static async Task CopyFile(CStorageEntityContract file)
        {
            var builder = new PostRequestBuilder<CStorageEntityContract>(_fileCopyRequest, file);

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

        public static async Task<CAccessWrapperContract> GetFile(Guid id)
        {
            var builder = new GetRequestBuilder(_fileRequest + $"{id}");

            return await HttpHelper.Request<CAccessWrapperContract>(builder, response =>
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

        public static async Task<CAccessWrapperContract> UploadFile(FileInfo info, Guid parentId,
            IProgress<Double> onProgressPercentChanged, Int32 blockSize = 10000)
        {
            Guid ownerId = AuthController.AuthenticatedUserUiModel.Id;

            CTransmissionEndPointContract endPoint = await GetEndpoint(_getUploadEndpointRequest);

            using TcpClient client = new TcpClient();
            await client.ConnectAsync(endPoint.Address, endPoint.Port);

            await using NetworkStream networkStream = client.GetStream();
            await networkStream.WriteStringAsync(info.Name);
            await networkStream.WriteGuidAsync(parentId);
            await networkStream.WriteStringAsync(AuthController.AuthenticatedUserUiModel.Token);
            Int64 size = info.Length;
            await networkStream.WriteInt64Async(size);

            await using (FileStream reader = File.OpenRead(info.FullName))
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

            Guid fileId = await networkStream.ReadGuidAsync();
            CAccessWrapperContract newFile = await GetFile(fileId);

            return newFile;
        }

        public static async Task LoadEntity(String path, IStorageEntityUiModel entity, 
            IProgress<Double> onProgressPercentChanged, Int32 blockSize = 10000)
        {
            Guid ownerId = AuthController.AuthenticatedUserUiModel.Id;
            Int64 size; 

            if (entity is CFileUiModel file)
            {
                size = file.Size;
            }
            else if (entity is CDirectoryUiModel dir)
            {
                size = 1;// TODO: Dir sizes
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
                    await networkStream.WriteGuidAsync(entity.Id);
                    await networkStream.WriteStringAsync(AuthController.AuthenticatedUserUiModel.Token);

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
