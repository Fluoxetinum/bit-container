using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Icons;
using BitContainer.Presentation.Models;

namespace BitContainer.Presentation.Helpers
{
    public class ContractsConverter
    {
        public static EAccessTypeUiModel ToUiAccessType(ERestrictedAccessTypeContract accessTypeContract)
        {
            switch (accessTypeContract)
            {
                case ERestrictedAccessTypeContract.Read:
                    return EAccessTypeUiModel.Read;
                case ERestrictedAccessTypeContract.Write:
                    return EAccessTypeUiModel.Write;
                default:
                    throw new InvalidOperationException("Not suppoert access type enum value.");
            }
        }

        public static ERestrictedAccessTypeContract ToAccessTypeContract(EAccessTypeUiModel accessTypeUiModel)
        {
            switch (accessTypeUiModel)
            {
                case EAccessTypeUiModel.Read:
                    return ERestrictedAccessTypeContract.Read;
                case EAccessTypeUiModel.Write:
                    return ERestrictedAccessTypeContract.Write;
                default:
                    throw new InvalidOperationException("Not suppoert access type enum value.");
            }
        }

        public static IStorageEntityUiModel GetStorageEntityUiModel(IStorageEntityContract entityContract)
        {
            IStorageEntityUiModel seModel = null;

            switch (entityContract)
            {
                case CFileContract fContract: 
                    seModel = new CFileUiModel(fContract);
                    break;
                case CDirectoryContract dContract :
                    seModel = new CDirectoryUiModel(dContract);
                    break;
                default:
                    throw new NotSupportedException("Not supported storage contract.");
            }

            return seModel;
        }

        public static IAccessWrapperUiModel GetAccessWrapperUiModel(IAccessWrapperContract wrapperContract)
        {
            IStorageEntityUiModel entityUiModel = GetStorageEntityUiModel(wrapperContract.EntityContract);

            switch (wrapperContract)
            {
                case COwnStorageEntityContract own:
                    return new COwnStorageEntityUiModel(entityUiModel, own.IsShared);
                case CRestrictedStorageEntityContract restricted:
                    EAccessTypeUiModel accessTypeUiModel = ToUiAccessType(restricted.AccessContract);
                    return new CRestrictedStorageEntityUiModel(entityUiModel, accessTypeUiModel);
                default:
                    throw new NotSupportedException("Not supported storage contract.");
            }
        }
        
        public static IStorageEntityContract GetStorageEntityContract(IStorageEntityUiModel uiModel)
        {
            switch (uiModel)
            {
                case CFileUiModel file:
                    return new CFileContract(file.Id, 
                        file.ParentId, 
                        file.OwnerId, 
                        file.Name, 
                        file.Created, 
                        file.Size);
                case CDirectoryUiModel dir:
                    return new CDirectoryContract(dir.Id,
                        dir.ParentId,
                        dir.OwnerId,
                        dir.Name,
                        dir.Created);
                default:
                    throw new NotSupportedException("Not supported storage entity ui model.");
            }
        }

    }
}
