using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Auth;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Models;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Helpers
{
    public class ContractsConverter
    {
        public static List<ISharableEntityUi> Convert(List<CSearchResultContract> contracts)
        {
            return contracts.Select(Convert).ToList();
        }

        public static ISharableEntityUi Convert(CSearchResultContract contract)
        {
            List<CStorageEntityId> parents = contract.Parents.Select(id => new CStorageEntityId(id)).ToList();
            return new CSearchResultUi(Convert(contract.SharableEntity), parents);
        }

        public static List<ISharableEntityUi> Convert(List<CSharableEntityContract> contracts)
        {
            return contracts.Select(Convert).ToList();
        }

        public static ISharableEntityUi Convert(CSharableEntityContract contract)
        {
            IStorageEntityUi entity = Convert(contract.AccessWrapper.EntityContract);
            EAccessType access = contract.AccessWrapper.Access;
            List<CShareUi> shares = Convert(contract.Shares);

            return new CSharableEntityUi(entity, access, shares);
        }

        public static IStorageEntityUi Convert(CStorageEntityContract contract)
        {
            CStorageEntityId id = new CStorageEntityId(contract.Id);
            CStorageEntityId parentId = new CStorageEntityId(contract.ParentId);
            CUserId ownerId = new CUserId(contract.OwnerId);
            switch (contract.Type)
            {
                case EStorageEntityTypeContract.Directory:
                    return new CDirectoryUi(id, parentId, ownerId, contract.Name, contract.Created);
                case EStorageEntityTypeContract.File:
                    return new CFileUi(id, parentId, ownerId, contract.Name, contract.Created, contract.Size);
                default:
                    throw new ArgumentException(nameof(contract.Type));
            }
        }

        public static CStorageEntityContract Convert(IStorageEntityUi entity)
        {
            switch (entity)
            {
                case CDirectoryUi dir:
                    return new CStorageEntityContract(EStorageEntityTypeContract.Directory,
                        dir.Id.ToGuid(),
                        dir.ParentId.ToGuid(),
                        dir.OwnerId.ToGuid(),
                        dir.Name,
                        dir.Created,
                        size:0);
                case CFileUi file:
                    return new CStorageEntityContract(EStorageEntityTypeContract.File,
                        file.Id.ToGuid(),
                        file.ParentId.ToGuid(),
                        file.OwnerId.ToGuid(),
                        file.Name,
                        file.Created,
                        file.Size);
                default:
                    throw new ArgumentException(nameof(entity));
            }
        }

        public static List<CShareUi> Convert(List<CShareContract> contracts)
        {
            return contracts.Select(Convert).ToList();
        }

        public static CShareUi Convert(CShareContract contract)
        {
            CStorageEntityId entityId = new CStorageEntityId(contract.EntityId);
            CUserId userId = new CUserId(contract.UserId);
            return new CShareUi(entityId, userId, contract.Access);
        }

        public static CUserUi Convert(CAuthenticatedUserContract contract)
        {
            CUserId userId = new CUserId(contract.User.Id);
            string name = contract.User.Name;
            Int32 filesCount = contract.Stats.FilesCount;
            Int32 dirsCount = contract.Stats.DirsCount;
            Int64 storageSize = contract.Stats.StorageSize;
            string token = contract.Token;

            return new CUserUi(userId, name, filesCount, dirsCount, storageSize, token);
        }
    }
}
