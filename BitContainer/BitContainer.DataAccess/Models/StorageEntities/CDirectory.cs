using System;

namespace BitContainer.DataAccess.Models.StorageEntities
{
    public class CDirectory : IStorageEntity
    {
        public Guid Id { get; }
        public Guid ParentId { get; }
        public Guid OwnerId { get; }
        public String Name { get; }
        public DateTime Created { get; }

        public CDirectory(Guid id, Guid parentId, Guid ownerId, String name, DateTime created)
        {
            Id = id;
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Created = created;
        }
    }
}
