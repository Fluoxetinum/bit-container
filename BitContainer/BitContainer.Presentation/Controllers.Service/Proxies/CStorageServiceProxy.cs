using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.ActionContracts;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Http;
using BitContainer.Http.Requests;
using BitContainer.Presentation.Controllers.Service;
using BitContainer.Presentation.Models;
using BitContainer.Shared.Models;
using BitContainer.Shared.StreamHelpers;

namespace BitContainer.Presentation.Controllers.Proxies
{
    public class CStorageServiceProxy
    {
        private static readonly String StorageControllerName = "storage";

        private readonly String _getOwnerStorageEntitiesRequest;
        private readonly String _getSharedStorageEntitiesRequest;

        private readonly String _searchRequest;
        private readonly String _searchSharedRequest;

        private readonly String _getUploadEndpointRequest;
        private readonly String _getDownloadEndpointRequest;

        private readonly String _fileRequest;
        private readonly String _shareRequest;
        private readonly String _dirRequest;
        private readonly String _entityRequest;
        private readonly String _copyRequest;
        
        private readonly IHttpHelper _httpHelper;

        public CStorageServiceProxy(IHttpHelper httpHelper, String serviceUrl)
        {
            _httpHelper = httpHelper;

            _getOwnerStorageEntitiesRequest = $@"{serviceUrl}/{StorageControllerName}/entities/";
            _getSharedStorageEntitiesRequest = $@"{serviceUrl}/{StorageControllerName}/shared_entities/";
            
            _searchRequest = $@"{serviceUrl}/{StorageControllerName}/entities/search/";
            _searchSharedRequest = $@"{serviceUrl}/{StorageControllerName}/shared_entities/search/";
            
            _getUploadEndpointRequest = $@"{serviceUrl}/{StorageControllerName}/upload_endpoint/";
            _getDownloadEndpointRequest = $@"{serviceUrl}/{StorageControllerName}/download_endpoint/";
            
            _fileRequest = $@"{serviceUrl}/{StorageControllerName}/file/";
            _shareRequest = $@"{serviceUrl}/{StorageControllerName}/share/";
            _dirRequest = $@"{serviceUrl}/{StorageControllerName}/dir/";
            _entityRequest = $@"{serviceUrl}/{StorageControllerName}/entity/";
            _copyRequest = $@"{serviceUrl}/{StorageControllerName}/copy/";
        }

        public async Task<List<CSharableEntityContract>> GetOwnerStorageEntites(Guid parentId)
        {
            var builder = new GetRequestBuilder(_getOwnerStorageEntitiesRequest + parentId);
            return await _httpHelper.Request<List<CSharableEntityContract>>(builder, ServiceErrorsCatcher);
        }

        public async Task<List<CSharableEntityContract>> GetSharedStorageEntites(Guid parentId)
        {
            var builder = new GetRequestBuilder(_getSharedStorageEntitiesRequest + parentId);
            return await _httpHelper.Request<List<CSharableEntityContract>>(builder, ServiceErrorsCatcher);
        }

        public async Task<List<CSearchResultContract>> SearchStorageEntities(Guid parentId, string pattern)
        {
            var builder = new GetRequestBuilder(_searchRequest + $"{parentId}/{pattern}");
            return await _httpHelper.Request<List<CSearchResultContract>>(builder, ServiceErrorsCatcher);
        }
        
        public async Task<List<CSearchResultContract>> SearchSharedStorageEntities(Guid parentId, string pattern)
        {
            var builder = new GetRequestBuilder(_searchSharedRequest + $"{parentId}/{pattern}");
            return await _httpHelper.Request<List<CSearchResultContract>>(builder, ServiceErrorsCatcher);
        }

        public async Task<CSharableEntityContract> GetFile(CStorageEntityId id)
        {
            var builder = new GetRequestBuilder(_fileRequest + $"{id}");
            return await _httpHelper.Request<CSharableEntityContract>(builder, ServiceErrorsCatcher);
        }

        public async Task UpdateShare(CNewShareContract newShare)
        {
            var builder = new PutRequestBuilder<CNewShareContract>(_shareRequest, newShare);
            await _httpHelper.Request(builder, ServiceErrorsCatcher);
        }

        public async Task<CSharableEntityContract> CreateDirectory(CNewDirContract dirContract)
        {
            var builder = new PostRequestBuilder<CNewDirContract>(_dirRequest, dirContract);
            return await _httpHelper.Request<CSharableEntityContract>(builder, ServiceErrorsCatcher);
        }

        public async Task DeleteEntity(Guid id)
        {
            var builder = new DeleteRequestBuilder<Guid>(_entityRequest, id);
            await _httpHelper.Request(builder, ServiceErrorsCatcher);
        }

        public async Task RenameEntity(CRenameEntityContract renameInfo)
        {
            var builder = new PutRequestBuilder<CRenameEntityContract>(_entityRequest, renameInfo);
            await _httpHelper.Request(builder, ServiceErrorsCatcher);
        }

        public async Task CopyEntity(CCopyEntityContract copyInfo)
        {
            var builder = new PostRequestBuilder<CCopyEntityContract>(_copyRequest, copyInfo);
            await _httpHelper.Request(builder, ServiceErrorsCatcher);
        }

        public async Task<CSharableEntityContract> UploadFile(FileInfo info, Guid parentId,
            IProgress<Double> onProgressPercentChanged, Int32 blockSize = 10000)
        {
            String token = CAuthController.CurrentUser.Token;

            CTransmissionEndPointContract endPoint = await GetEndpoint(_getUploadEndpointRequest);

            using TcpClient client = new TcpClient();
            await client.ConnectAsync(endPoint.Address, endPoint.Port);

            await using NetworkStream networkStream = client.GetStream();
            await networkStream.WriteStringAsync(info.Name);
            await networkStream.WriteGuidAsync(parentId);
            await networkStream.WriteStringAsync(token);
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
            CSharableEntityContract newFile = await GetFile(new CStorageEntityId(fileId));

            return newFile;
        }

        public async Task LoadEntity(String path, IStorageEntityUi entity, 
            IProgress<Double> onProgressPercentChanged, Int32 blockSize = 10000)
        {
            String token = CAuthController.CurrentUser.Token;

            Int64 size; 

            if (entity is CFileUi file)
            {
                size = file.Size;
            }
            else if (entity is CDirectoryUi dir)
            {
                size = 1;// TODO: Dir sizes
                path += ".zip";
            }
            else
            {
                throw new NotSupportedException("not supported entity type.");
            }
            
            CTransmissionEndPointContract endPoint = await GetEndpoint(_getDownloadEndpointRequest);

            using TcpClient client = new TcpClient();
            await client.ConnectAsync(endPoint.Address, endPoint.Port);

            await using NetworkStream networkStream = client.GetStream();
            await networkStream.WriteGuidAsync(entity.Id.ToGuid());
            await networkStream.WriteStringAsync(token);

            await using FileStream writer = File.OpenWrite(path);
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
        
        private async Task<CTransmissionEndPointContract> GetEndpoint(String request)
        {
            var builder = new GetRequestBuilder(request);
            return await _httpHelper.Request<CTransmissionEndPointContract>(builder, ServiceErrorsCatcher);
        }

        private void ServiceErrorsCatcher(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
