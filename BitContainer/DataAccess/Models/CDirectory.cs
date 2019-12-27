using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Models
{
    public class CDirectory : IStorageEntity
    {
        public Guid Id { get; }
        public Guid ParentId { get; }
        public Guid OwnerId { get; }
        public String Name { get; }
        public DateTime Created { get; }
        public Boolean IsShared { get; }

        public CDirectory(Guid id, Guid parentId, Guid ownerId, String name, DateTime created, Boolean isShared = false)
        {
            Id = id;
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Created = created;
            IsShared = isShared;
        }
    }
}
