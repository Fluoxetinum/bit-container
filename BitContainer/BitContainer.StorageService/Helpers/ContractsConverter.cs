using System;
using System.Collections.Generic;
using System.Linq;
using BitContainer.Contracts.V1.Auth;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;

namespace BitContainer.Service.Storage.Helpers
{
    public static class ContractsConverter
    {
        public static CStorageEntityContract Convert(IStorageEntity entity)
        {
            switch (entity)
            {
                case CFile file:
                    return new CStorageEntityContract(EStorageEntityTypeContract.File, 
                        file.Id.ToGuid(), 
                        file.ParentId.ToGuid(), 
                        file.OwnerId.ToGuid(), 
                        file.Name, 
                        file.Created, 
                        file.Size);
                case CDirectory dir:
                    return new CStorageEntityContract(EStorageEntityTypeContract.Directory, 
                        dir.Id.ToGuid(), 
                        dir.ParentId.ToGuid(), 
                        dir.OwnerId.ToGuid(), 
                        dir.Name, 
                        dir.Created, 
                        size:0);
                default:
                    throw new InvalidCastException(nameof(entity));
            }
        }

        public static CShareContract Convert(CShare share)
        {
            return new CShareContract(share.EntityId.ToGuid(), share.UserId.ToGuid(), share.AccessType);
        }

        public static List<CShareContract> Convert(List<CShare> shares)
        {
            return shares.Select(Convert).ToList();
        }

        public static CAccessWrapperContract Convert(CAccessWrapper wrapper)
        {
            return new CAccessWrapperContract(Convert(wrapper.Entity), wrapper.AcesssType);
        }

        public static CSharableEntityContract Convert(CSharableEntity entity)
        {
            return new CSharableEntityContract(Convert(entity.AccessWrapper), Convert(entity.Shares));
        }

        public static List<CSharableEntityContract> Convert(List<CSharableEntity> entities)
        {
            return entities.Select(Convert).ToList();
        }

        public static List<CSearchResultContract> Convert(List<CSearchResult> searchResults)
        {
            List<CSearchResultContract> contracts = new List<CSearchResultContract>();

            foreach (var result in searchResults)
            {
                CSharableEntityContract sharableEntity = Convert(result.SharableEntity);

                List<Guid> parents = result.Parents.Select(id => id.ToGuid()).ToList();
                
                contracts.Add(new CSearchResultContract(sharableEntity, parents));
            }

            return contracts;
        }

    }
}
