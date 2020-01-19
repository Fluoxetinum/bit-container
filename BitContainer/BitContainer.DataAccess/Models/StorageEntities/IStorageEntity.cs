using System;

namespace BitContainer.DataAccess.Models.StorageEntities
{
    public interface IStorageEntity
    {
        Guid Id { get; }
        Guid ParentId { get;  }
        Guid OwnerId { get;  }
        String Name { get;  }
        DateTime Created { get; }
    } 
}
