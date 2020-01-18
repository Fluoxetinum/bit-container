using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.Storage
{
    public class CStorageEntityContract
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid OwnerId { get; set; }
        public String Name { get; set; }
        public DateTime Created { get; set; }

        public Int64? Size { get; set; }
        public Boolean IsFile => Size.HasValue;
        public Boolean IsDirectory => !Size.HasValue;

        public CStorageEntityContract(Guid id, Guid parentId, Guid ownerId, String name, DateTime created, Int64? size = null)
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
