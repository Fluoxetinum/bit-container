using System;
using System.Collections.Generic;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.DataAccess.Models;

namespace BitContainer.StorageService.Helpers
{
    public static class ContractsConverter
    {
        public static ERestrictedAccessTypeContract ToAccessTypeContract(this ERestrictedAccessType restrictedAccess)
        {
            switch (restrictedAccess)
            {
                case ERestrictedAccessType.Read:
                    return ERestrictedAccessTypeContract.Read;
                case ERestrictedAccessType.Write:
                    return ERestrictedAccessTypeContract.Write;
                case ERestrictedAccessType.None:
                    return ERestrictedAccessTypeContract.None;
                default:
                    throw new NotSupportedException("Invalid ERestrictedAccessType value.");
            }
        }

        public static ERestrictedAccessType ToAccessType(this ERestrictedAccessTypeContract restrictedAccessContract)
        {
            switch (restrictedAccessContract)
            {
                case ERestrictedAccessTypeContract.Read:
                    return ERestrictedAccessType.Read;
                case ERestrictedAccessTypeContract.Write:
                    return ERestrictedAccessType.Write;
                case ERestrictedAccessTypeContract.None:
                    return ERestrictedAccessType.None;
                default:
                    throw new NotSupportedException("Invalid ERestrictedAccessTypeContract value.");
            }
        }

        public static IStorageEntityContract ToStorageEntityContract(IStorageEntity entity)
        {
            switch (entity)
            {
                case CFile f:
                    return new CFileContract(f.Id, f.ParentId, f.OwnerId, f.Name, f.Created, f.Size);
                case CDirectory d:
                    return new CDirectoryContract(d.Id, d.ParentId, d.OwnerId, d.Name, d.Created);
                default:
                    throw new NotSupportedException("Invalid IStorageEntity implementation");
            }
        }

        private static CRestrictedStorageEntityContract ToRestrictedStorageEntityContract
            (IStorageEntityContract entityContract, ERestrictedAccessTypeContract accessContract)
        {
            return new CRestrictedStorageEntityContract(entityContract, accessContract);
        }

        public static CRestrictedStorageEntityContract ToRestrictedStorageEntityContract(IStorageEntity entity, ERestrictedAccessType access)
        {
            IStorageEntityContract c = ToStorageEntityContract(entity);
            return ToRestrictedStorageEntityContract(c, access.ToAccessTypeContract());
        }

        private static COwnStorageEntityContract ToOwnStorageEntityContract(IStorageEntityContract entityContract, Boolean isShared)
        {
            return new COwnStorageEntityContract(entityContract, isShared);
        }

        public static COwnStorageEntityContract ToOwnStorageEntityContract(IStorageEntity entity, Boolean isShared)
        {
            IStorageEntityContract c = ToStorageEntityContract(entity);
            return ToOwnStorageEntityContract(c, isShared);
        }

        private static List<COwnStorageEntityContract> ToOwnStorageEntityContracts(params List<COwnStorageEntity>[] lists)
        {
            List<COwnStorageEntityContract> contract = new List<COwnStorageEntityContract>();

            foreach (var list in lists)
            {
                foreach (var cOwnStorageEntity in list)
                {
                    contract.Add(ToOwnStorageEntityContract(cOwnStorageEntity.Entity, cOwnStorageEntity.IsShared));
                }
            }
            return contract;
        }

        private static List<CRestrictedStorageEntityContract> ToRestrictedEntityContracts(params List<CRestrictedStorageEntity>[] lists)
        {
            List<CRestrictedStorageEntityContract> contract = new List<CRestrictedStorageEntityContract>();

            foreach (var list in lists)
            {
                foreach (var cRestrictedStorageEntity in list)
                {
                    contract.Add(ToRestrictedStorageEntityContract(cRestrictedStorageEntity.Entity, cRestrictedStorageEntity.RestrictedAccess));
                }
            }
            return contract;
        }

        public static COwnStorageEntitiesListContract ToOwnStorageEntitiesListContract(params List<COwnStorageEntity>[] lists)
        {
            return new COwnStorageEntitiesListContract(ToOwnStorageEntityContracts(lists));
        }

        public static CRestrictedStorageEntitiesListContract ToRestrictedEntitiesListContract(params List<CRestrictedStorageEntity>[] lists)
        {
            return new CRestrictedStorageEntitiesListContract(ToRestrictedEntityContracts(lists));
        }

        public static List<CSearchResultContract> ToSearchResultContractList(List<CSearchResult> list)
        {
            List<CSearchResultContract> searchResult = new List<CSearchResultContract>();

            foreach (var result in list)
            {
                LinkedList<Guid> path = result.DownPath;

                IStorageEntityContract storageEntity = ToStorageEntityContract(result.AccessWrapper.Entity);

                IAccessWrapperContract accessWrapper;
                switch (result.AccessWrapper)
                {
                    case CRestrictedStorageEntity re:
                        accessWrapper = ToRestrictedStorageEntityContract(storageEntity,
                            re.RestrictedAccess.ToAccessTypeContract());
                        break;
                    case COwnStorageEntity oe:
                        accessWrapper = ToOwnStorageEntityContract(storageEntity, oe.IsShared);
                        break;
                    default:
                        throw new NotSupportedException("Not supported access wrapper type");
                }

                searchResult.Add(new CSearchResultContract()
                {
                    AccessWrapper = accessWrapper,
                    DownPath = path
                });
            }

            return searchResult;
        }


    }
}
