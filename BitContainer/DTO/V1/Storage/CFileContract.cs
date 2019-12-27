using System;

namespace BitContainer.Contracts.V1.Storage
{
    public class CFileContract : IStorageEntityContract
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid OwnerId { get; set; }
        public String Name { get; set; }
        public DateTime Created { get; set; }

        public Int64 Size { get; set; }

        public CFileContract(){}

        public CFileContract(Guid id, Guid parentId, Guid ownerId, String name, DateTime created, Int64 size)
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
