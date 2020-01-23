using System;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Models.StorageEntities
{
    public class CDirectory : IStorageEntity
    {
        public CStorageEntityId Id { get; }
        public CStorageEntityId ParentId { get; }
        public CUserId OwnerId { get; }
        public String Name { get; }
        public DateTime Created { get; }

        public CDirectory(CStorageEntityId id, CStorageEntityId parentId, CUserId ownerId, String name, DateTime created)
        {
            Id = id;
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Created = created;
        }

        public static CDirectory Root = new CDirectory();
        private CDirectory() => Id = CStorageEntityId.RootId; 
    }
}
