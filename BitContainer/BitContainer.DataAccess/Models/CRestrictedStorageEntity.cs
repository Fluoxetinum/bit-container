using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess.Models.StorageEntities;

namespace BitContainer.DataAccess.Models
{
    public class CRestrictedStorageEntity : IAccessWrapper
    {
        public IStorageEntity Entity { get;  }

        public ERestrictedAccessType RestrictedAccess { get; }

        public CRestrictedStorageEntity(IStorageEntity entity, ERestrictedAccessType restrictedAccess)
        {
            Entity = entity;
            RestrictedAccess = restrictedAccess;
        }
    }
}
