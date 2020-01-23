using System;
using BitContainer.Shared.Models;


namespace BitContainer.DataAccess.Models.StorageEntities
{
    public class CFile : IStorageEntity
    {
        public CStorageEntityId Id { get; }
        public CStorageEntityId ParentId { get; }
        public CUserId OwnerId { get; }
        public String Name { get; }
        public DateTime Created { get; }

        public Int64 Size { get; }

        public CFile(CStorageEntityId id,
            CStorageEntityId parentId,
            CUserId ownerId,
            String name,
            DateTime created,
            Int64 size)
        {
            Id = id;
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Created = created;
            Size = size;
        }

    }
}
