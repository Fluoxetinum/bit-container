using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BitContainer.Contracts.V1.ActionContracts;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Http.Exceptions;
using BitContainer.Presentation.Controllers.Proxies;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.Helpers;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Jobs;
using BitContainer.Presentation.Views.Dialogs;
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
            
            await HandleErrors(async () =>
            {
                result = await _cStorageServiceProxy.GetOwnerStorageEntites(parentId.ToGuid());
            });

            return ContractsConverter.Convert(result);
        }

        public async Task<List<ISharableEntityUi>> GetSharedStorageEntities(CStorageEntityId parentId)
        {
            List<CSharableEntityContract> result = null;

            await HandleErrors(async () =>
            {
                result = await _cStorageServiceProxy.GetSharedStorageEntites(parentId.ToGuid());
            });

            return ContractsConverter.Convert(result);
        }

        public async Task<List<ISharableEntityUi>> SearchOwnerEntities(CStorageEntityId parentId, string pattern)
        {
            List<CSearchResultContract> result = null;

            await HandleErrors(async () =>
            {
                result = await _cStorageServiceProxy.SearchStorageEntities(parentId.ToGuid(), pattern);
            });

            return ContractsConverter.Convert(result);
        }

        public async Task<List<ISharableEntityUi>> SearchSharedEntities(CStorageEntityId parentId, string pattern)
        {
            List<CSearchResultContract> result = null;

            await HandleErrors(async () =>
            {
                result = await _cStorageServiceProxy.SearchSharedStorageEntities(parentId.ToGuid(), pattern);
            });

            return ContractsConverter.Convert(result);
        }

        public async Task UpdateShare(String userName, EAccessType access, IStorageEntityUi uiEntity)
        {
            CNewShareContract newNewShare = new CNewShareContract(userName, access, uiEntity.Id.ToGuid());
            await HandleErrors(async () =>
            {
                await _cStorageServiceProxy.UpdateShare(newNewShare);
            });
        }

        public async Task<ISharableEntityUi> CreateDirectory(String name, CStorageEntityId parentId)
        {
            CNewDirContract contract = new CNewDirContract(parentId.ToGuid(), name);
            
            CSharableEntityContract result = null;

            Boolean res = await HandleErrors(async () =>
            {
                result = await _cStorageServiceProxy.CreateDirectory(contract);
            });

            if (!res) return null;

            return ContractsConverter.Convert(result);
        }

        public async Task DeleteEntity(CStorageEntityId id)
        {
            await HandleErrors(async () =>
            {
                await _cStorageServiceProxy.DeleteEntity(id.ToGuid());
            });
        }

        public async Task RenameEntity(CStorageEntityId id, String newName)
        {
            CRenameEntityContract renameContract = new CRenameEntityContract(id.ToGuid(), newName);

            await HandleErrors(async () =>
            {
                await _cStorageServiceProxy.RenameEntity(renameContract);
            });
        }

        public async Task CopyEntity(CStorageEntityId entityId, CStorageEntityId newParentId)
        {
            CCopyEntityContract copyContract = new CCopyEntityContract(entityId.ToGuid(), newParentId.ToGuid());

            await HandleErrors(async () =>
            {
                await _cStorageServiceProxy.CopyEntity(copyContract);
            });
        }

        public async Task<ISharableEntityUi> UploadFile(String path, CStorageEntityId parentId)
        {
            CSharableEntityContract file = null;
            FileInfo info = new FileInfo(path);
            UploadJob job = UploadJob.Create(info.Name);
            var progress = new Progress<Double>(i => job.Progress = i);
            AbstractJob.NotifyJobCreated(job);

            Boolean res = await HandleErrors(async () =>
            {
                file = await _cStorageServiceProxy.UploadFile(info, parentId.ToGuid(), progress);
            });

            if (!res) return null;

            return ContractsConverter.Convert(file);
        }

        public async Task LoadEntity(String path, IStorageEntityUi entity)
        {
            DownloadJob job = DownloadJob.Create(entity.Name);
            var progress = new Progress<double>(i => job.Progress = i);
            AbstractJob.NotifyJobCreated(job);
            await _cStorageServiceProxy.LoadEntity(path, entity, progress);
        }

        public static async Task<Boolean> HandleErrors(Func<Task> action)
        {
            try
            {
                await action();
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                NavigationController.GoToLoginPage();
                return false;
            }
            catch (StorageOperationCanceledException e)
            {
                ErrorDialog dialog = new ErrorDialog(e.Message)
                {
                    Title = "Error",
                    ResizeMode = ResizeMode.NoResize,
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                dialog.ShowDialog();
                return false;
            }
        }


    }
}
