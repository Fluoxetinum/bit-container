using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BitContainer.Contracts.V1.ActionContracts;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Controllers.Proxies;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.Helpers;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Jobs;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Controllers.Service
{
    public class CStorageController
    {
        private readonly CStorageServiceProxy _cStorageServiceProxy;
        
        public CStorageController(CStorageServiceProxy cStorageServiceProxy)
        {
            _cStorageServiceProxy = cStorageServiceProxy;
        }

        public async Task<List<ISharableEntityUi>> GetOwnerStorageEntities(CStorageEntityId parentId)
        {
            List<CSharableEntityContract> result = null;
            
            await HandleAuthError(async () =>
            {
                result = await _cStorageServiceProxy.GetOwnerStorageEntites(parentId.ToGuid());
            });

            return ContractsConverter.Convert(result);
        }

        public async Task<List<ISharableEntityUi>> GetSharedStorageEntities(CStorageEntityId parentId)
        {
            List<CSharableEntityContract> result = null;

            await HandleAuthError(async () =>
            {
                result = await _cStorageServiceProxy.GetSharedStorageEntites(parentId.ToGuid());
            });

            return ContractsConverter.Convert(result);
        }

        public async Task<List<ISharableEntityUi>> SearchOwnerEntities(CStorageEntityId parentId, string pattern)
        {
            List<CSearchResultContract> result = null;

            await HandleAuthError(async () =>
            {
                result = await _cStorageServiceProxy.SearchStorageEntities(parentId.ToGuid(), pattern);
            });

            return ContractsConverter.Convert(result);
        }

        public async Task<List<ISharableEntityUi>> SearchSharedEntities(CStorageEntityId parentId, string pattern)
        {
            List<CSearchResultContract> result = null;

            await HandleAuthError(async () =>
            {
                result = await _cStorageServiceProxy.SearchSharedStorageEntities(parentId.ToGuid(), pattern);
            });

            return ContractsConverter.Convert(result);
        }

        public async Task UpdateShare(String userName, EAccessType access, IStorageEntityUi uiEntity)
        {
            CNewShareContract newNewShare = new CNewShareContract(userName, access, uiEntity.Id.ToGuid());
            await HandleAuthError(async () =>
            {
                await _cStorageServiceProxy.UpdateShare(newNewShare);
            });
        }

        public async Task<ISharableEntityUi> CreateDirectory(String name, CStorageEntityId parentId)
        {
            CNewDirContract contract = new CNewDirContract(parentId.ToGuid(), name);
            
            CSharableEntityContract result = null;

            await HandleAuthError(async () =>
            {
                result = await _cStorageServiceProxy.CreateDirectory(contract);
            });

            return ContractsConverter.Convert(result);
        }

        public async Task DeleteEntity(CStorageEntityId id)
        {
            await HandleAuthError(async () =>
            {
                await _cStorageServiceProxy.DeleteEntity(id.ToGuid());
            });
        }

        public async Task RenameEntity(CStorageEntityId id, String newName)
        {
            CRenameEntityContract renameContract = new CRenameEntityContract(id.ToGuid(), newName);

            await HandleAuthError(async () =>
            {
                await _cStorageServiceProxy.RenameEntity(renameContract);
            });
        }

        public async Task CopyEntity(CStorageEntityId entityId, CStorageEntityId newParentId)
        {
            CCopyEntityContract copyContract = new CCopyEntityContract(entityId.ToGuid(), newParentId.ToGuid());

            await HandleAuthError(async () =>
            {
                await _cStorageServiceProxy.CopyEntity(copyContract);
            });
        }

        public  async Task<ISharableEntityUi> UploadFile(String path, CStorageEntityId parentId)
        {
            CSharableEntityContract file = null;
            FileInfo info = new FileInfo(path);
            UploadJob job = UploadJob.Create(info.Name);
            var progress = new Progress<Double>(i => job.Progress = i);
            AbstractJob.NotifyJobCreated(job);

            await HandleAuthError(async () =>
            {
                file = await _cStorageServiceProxy.UploadFile(info, parentId.ToGuid(), progress);
            });

            return ContractsConverter.Convert(file);
        }

        public async Task LoadEntity(String path, IStorageEntityUi entity)
        {
            DownloadJob job = DownloadJob.Create(entity.Name);
            var progress = new Progress<double>(i => job.Progress = i);
            AbstractJob.NotifyJobCreated(job);
            await _cStorageServiceProxy.LoadEntity(path, entity, progress);
        }

        public static async Task HandleAuthError(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (UnauthorizedAccessException)
            {
                NavigationController.GoToLoginPage();
            }
        }


    }
}
