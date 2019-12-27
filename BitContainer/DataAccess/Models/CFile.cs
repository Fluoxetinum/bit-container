using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BitContainer.DataAccess.Models
{
    public class CFile : IStorageEntity
    {
        public Guid Id { get; }
        public Guid ParentId { get; }
        public Guid OwnerId { get; }
        public String Name { get; }
        public DateTime Created { get; }

        public Int32 Size { get; }

        public CFile(Guid id,
            Guid parentId,
            Guid ownerId,
            String name,
            DateTime created,
            Int32 size)
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
