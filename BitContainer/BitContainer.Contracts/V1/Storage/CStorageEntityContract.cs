using System;
using BitContainer.Shared.Models;

namespace BitContainer.Contracts.V1.Storage
{
    public class CStorageEntityContract
    {
        public EStorageEntityTypeContract Type { get; set; }
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid OwnerId { get; set; }
        public String Name { get; set; }
        public DateTime Created { get; set; }
        public Int64 Size { get; set; }

        public CStorageEntityContract() { }

        public CStorageEntityContract(EStorageEntityTypeContract type, Guid id, Guid parentId, Guid ownerId, String name, DateTime created, Int64 size)
        {
            Type = type;
            Id = id;
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Created = created;
            Size = size;
        }
    }
}
