using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Controllers.Proxies;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.Helpers;
using BitContainer.Presentation.Icons;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Enums;
using BitContainer.Presentation.ViewModels.Jobs;

namespace BitContainer.Presentation.Controllers
{
    public static class StorageController
    {
        public static async Task<List<IAccessWrapperUiModel>> SearchOwnerEntities(Guid parentId, String pattern)
        {
            List<CSearchResultContract> result = null;

            await HandleAuthError(async () =>
                {
                    result = await StorageServiceProxy.SearchStorageEntities(parentId, pattern);
                });

            List<IAccessWrapperUiModel> list = new List<IAccessWrapperUiModel>();

            foreach (var d in result)
            {
                if (d.AccessWrapper.Access == ERestrictedAccessTypeContract.Full)
                {
                    Boolean isShared = d.AccessWrapper.HasShares;
                    IStorageEntityUiModel seModel = ContractsConverter.GetStorageEntityUiModel(d.AccessWrapper.EntityContract);
                    IAccessWrapperUiModel restrictedModel = new COwnStorageEntityUiModel(seModel, isShared);

                    list.Add(new CSearchResultUiModelDirtyAdapter(restrictedModel, d.DownPath));
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            return list;
        }

        public static async Task<List<IAccessWrapperUiModel>> SearchSharedEntities(Guid entityParentId, string pattern)
        {
            List<CSearchResultContract> result = null;

            await HandleAuthError(async () =>
            {
                result = await StorageServiceProxy.SearchSharedStorageEntities(entityParentId, pattern);
            });

            List<IAccessWrapperUiModel> list = new List<IAccessWrapperUiModel>();

            foreach (var d in result)
            {
                if (d.AccessWrapper.Access != ERestrictedAccessTypeContract.Full)
                {
                    EAccessTypeUiModel access = ContractsConverter.ToUiAccessType(d.AccessWrapper.Access);
                    IStorageEntityUiModel seModel = ContractsConverter.GetStorageEntityUiModel(d.AccessWrapper.EntityContract);
                    IAccessWrapperUiModel restrictedModel = new CRestrictedStorageEntityUiModel(seModel, access);

                    list.Add(new CSearchResultUiModelDirtyAdapter(restrictedModel, d.DownPath));
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            return list;
        }

        public static async Task<List<IAccessWrapperUiModel>> GetOwnerStorageEntities(Guid parentId)
        {
            List<CAccessWrapperContract> result = null;
            
            await HandleAuthError(async () =>
            {
                result = await StorageServiceProxy.GetOwnerStorageEntites(parentId);
            });

            List<IAccessWrapperUiModel> list = new List<IAccessWrapperUiModel>();

            foreach (var d in result)
            {
                Boolean isShared = d.HasShares;
                IStorageEntityUiModel seModel = ContractsConverter.GetStorageEntityUiModel(d.EntityContract);
                list.Add(new COwnStorageEntityUiModel(seModel, isShared));
            }

            return list;
        }

        public static async Task<List<IAccessWrapperUiModel>> GetSharedStorageEntities(Guid parentId)
        {
            List<CAccessWrapperContract> result = null;

            await HandleAuthError(async () =>
            {
                result = await StorageServiceProxy.GetSharedStorageEntites(parentId);
            });

            List<IAccessWrapperUiModel> list = new List<IAccessWrapperUiModel>();

            foreach (var d in result)
            {
                EAccessTypeUiModel access = ContractsConverter.ToUiAccessType(d.Access);
                IStorageEntityUiModel seModel = ContractsConverter.GetStorageEntityUiModel(d.EntityContract);
                list.Add(new CRestrictedStorageEntityUiModel(seModel, access));
            }

            return list;
        }

        public static async Task<IAccessWrapperUiModel> CreateDirectory(String name, Guid parentId)
        {
            CStorageEntityContract contract = new CStorageEntityContract(id:Guid.Empty, parentId:parentId, 
                ownerId:Guid.Empty, name:name, created:DateTime.MaxValue);
            
            CAccessWrapperContract result = null;

            await HandleAuthError(async () =>
            {
                result = await StorageServiceProxy.CreateDirectory(contract);
            });

            return ContractsConverter.GetAccessWrapperUiModel(result);
        }

        public static async Task<IAccessWrapperUiModel> UploadFile(String path, Guid parentId)
        {
            CAccessWrapperContract file = null;
            FileInfo info = new FileInfo(path);
            UploadJob job = UploadJob.Create(info.Name);
            var progress = new Progress<Double>(i => job.Progress = i);
            AbstractJob.NotifyJobCreated(job);

            await HandleAuthError(async () =>
            {
                file = await StorageServiceProxy.UploadFile(info, parentId, progress);
            });

            return ContractsConverter.GetAccessWrapperUiModel(file);
        }

        public static async Task LoadEntity(String path, IStorageEntityUiModel entity)
        {
            DownloadJob job = DownloadJob.Create(entity.Name);
            var progress = new Progress<Double>(i => job.Progress = i);
            AbstractJob.NotifyJobCreated(job);
            await StorageServiceProxy.LoadEntity(path, entity, progress);
        }

        public static async Task DeleteDirectory(CDirectoryUiModel dir)
        {
            CStorageEntityContract dirContract = ContractsConverter.GetStorageEntityContract(dir) ;

            await HandleAuthError(async () =>
            {
                await StorageServiceProxy.DeleteDirectory(dirContract);
            });
        }

        public static async Task DeleteFile(CFileUiModel file)
        {
            CStorageEntityContract fileContract = ContractsConverter.GetStorageEntityContract(file);

            await HandleAuthError(async () =>
            {
                await StorageServiceProxy.DeleteFile(fileContract);
            });
        }

        public static async Task RenameDirectory(CDirectoryUiModel renamedDir)
        {
            CStorageEntityContract dirContract = ContractsConverter.GetStorageEntityContract(renamedDir);

            await HandleAuthError(async () =>
            {
                await StorageServiceProxy.RenameDirectory(dirContract);
            });
        }

        public static async Task RenameFile(CFileUiModel renamedFile)
        {
            CStorageEntityContract fileContract = ContractsConverter.GetStorageEntityContract(renamedFile);

            await HandleAuthError(async () =>
            {
                await StorageServiceProxy.RenameFile(fileContract);
            });
        }

        public static async Task CopyDir(CDirectoryUiModel movedDir)
        {
            CStorageEntityContract dirContract = ContractsConverter.GetStorageEntityContract(movedDir);

            await HandleAuthError(async () =>
            {
                await StorageServiceProxy.CopyDirectory(dirContract);
            });
        }

        public static async Task CopyFile(CFileUiModel movedFile)
        {
            CStorageEntityContract fileContract =
                ContractsConverter.GetStorageEntityContract(movedFile);

            await HandleAuthError(async () =>
            {
                await StorageServiceProxy.CopyFile(fileContract);
            });
        }

        public static async Task UpdateShare(String userName, EAccessTypeUiModel access, IStorageEntityUiModel uiEntity)
        {
            ERestrictedAccessTypeContract accessContract = ContractsConverter.ToAccessTypeContract(access);
            CStorageEntityContract entityContract = ContractsConverter.GetStorageEntityContract(uiEntity);

            CNewShareContract newNewShare = CNewShareContract.Create(userName, accessContract, entityContract.Id);

            await HandleAuthError(async () =>
            {
                await StorageServiceProxy.UpdateShare(newNewShare);
            });
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
