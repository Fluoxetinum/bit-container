using System;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Models.StorageEntities
{
    public interface IStorageEntity
    {
        CStorageEntityId Id { get; }
        CStorageEntityId ParentId { get;  }
        CUserId OwnerId { get; }
        String Name { get; }
        DateTime Created { get; }
    } 
}
