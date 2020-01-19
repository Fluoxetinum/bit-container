using System;
using System.Collections.Generic;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;

namespace BitContainer.StorageService.Helpers
{
    public static class ContractsConverter
    {
        private static CAccessWrapperContract ToRestrictedStorageEntityContract(CStorageEntityContract entityContract, ERestrictedAccessTypeContract accessContract)
        {
            return new CAccessWrapperContract(entityContract, accessContract, hasShares:true);
        }

        private static CAccessWrapperContract ToOwnStorageEntityContract(CStorageEntityContract entityContract, Boolean hasShares)
        {
            return new CAccessWrapperContract(entityContract, ERestrictedAccessTypeContract.Full, hasShares);
        }

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

        public static CStorageEntityContract ToStorageEntityContract(IStorageEntity entity)
        {
            switch (entity)
            {
                case CFile f:
                    return new CStorageEntityContract(f.Id, f.ParentId, f.OwnerId, f.Name, f.Created, f.Size);
                case CDirectory d:
                    return new CStorageEntityContract(d.Id, d.ParentId, d.OwnerId, d.Name, d.Created);
                default:
                    throw new NotSupportedException("Invalid IStorageEntity implementation");
            }
        }

        public static CAccessWrapperContract ToRestrictedStorageEntityContract(IStorageEntity entity, ERestrictedAccessType access)
        {
            CStorageEntityContract c = ToStorageEntityContract(entity);
            return ToRestrictedStorageEntityContract(c, access.ToAccessTypeContract());
        }

        public static CAccessWrapperContract ToOwnStorageEntityContract(IStorageEntity entity, Boolean hasShares)
        {
            CStorageEntityContract c = ToStorageEntityContract(entity);
            return ToOwnStorageEntityContract(c, hasShares);
        }

        public static List<CAccessWrapperContract> ToOwnStorageEntityContracts(params List<COwnStorageEntity>[] lists)
        {
            List<CAccessWrapperContract> contract = new List<CAccessWrapperContract>();

            foreach (var list in lists)
            {
                foreach (var cOwnStorageEntity in list)
                {
                    contract.Add(ToOwnStorageEntityContract(cOwnStorageEntity.Entity, cOwnStorageEntity.IsShared));
                }
            }
            return contract;
        }

        public static List<CAccessWrapperContract> ToRestrictedStorageEntityContracts(params List<CRestrictedStorageEntity>[] lists)
        {
            List<CAccessWrapperContract> contract = new List<CAccessWrapperContract>();

            foreach (var list in lists)
            {
                foreach (var cRestrictedStorageEntity in list)
                {
                    contract.Add(ToRestrictedStorageEntityContract(cRestrictedStorageEntity.Entity, cRestrictedStorageEntity.RestrictedAccess));
                }
            }
            return contract;
        }

        public static List<CSearchResultContract> ToSearchResultContractList(List<CSearchResult> list)
        {
            List<CSearchResultContract> searchResult = new List<CSearchResultContract>();

            foreach (var result in list)
            {
                LinkedList<Guid> path = result.DownPath;

                CStorageEntityContract storageEntity = ToStorageEntityContract(result.AccessWrapper.Entity);

                CAccessWrapperContract accessWrapper;
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
