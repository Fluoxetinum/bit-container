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

        public static IStorageEntityUiModel GetStorageEntityUiModel(CStorageEntityContract entityContract)
        {
            IStorageEntityUiModel seModel = null;

            if (entityContract.Size.HasValue)
            {
                seModel = new CFileUiModel(entityContract.Id, entityContract.ParentId, entityContract.OwnerId,
                    entityContract.Name, entityContract.Created, entityContract.Size.Value);
            }
            else
            {
                seModel = new CDirectoryUiModel(entityContract.Id, entityContract.ParentId, entityContract.OwnerId,
                    entityContract.Name, entityContract.Created);
            }

            return seModel;
        }

        public static IAccessWrapperUiModel GetAccessWrapperUiModel(CAccessWrapperContract wrapperContract)
        {
            IStorageEntityUiModel entityUiModel = GetStorageEntityUiModel(wrapperContract.EntityContract);

            if (wrapperContract.Access == ERestrictedAccessTypeContract.Full)
                return new COwnStorageEntityUiModel(entityUiModel, wrapperContract.HasShares);
            else
            {
                EAccessTypeUiModel accessTypeUiModel = ToUiAccessType(wrapperContract.Access);
                return new CRestrictedStorageEntityUiModel(entityUiModel, accessTypeUiModel);
            }
        }
        
        public static CStorageEntityContract GetStorageEntityContract(IStorageEntityUiModel uiModel)
        {
            switch (uiModel)
            {
                case CFileUiModel file:
                    return new CStorageEntityContract(file.Id, 
                        file.ParentId, 
                        file.OwnerId, 
                        file.Name, 
                        file.Created, 
                        file.Size);
                case CDirectoryUiModel dir:
                    return new CStorageEntityContract(dir.Id,
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
