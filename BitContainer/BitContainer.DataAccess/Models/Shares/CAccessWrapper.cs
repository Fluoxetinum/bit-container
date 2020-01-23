using System;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Models.Shares
{
    public class CAccessWrapper
    {
        public IStorageEntity Entity { get; }
        public EAccessType AcesssType { get; }

        public CAccessWrapper(IStorageEntity entity, EAccessType access)
        {
            Entity = entity;
            AcesssType = access;
        }

        public override bool Equals(object obj)
        {
            return Entity.Id.Equals(Entity.Id);
        }

        public override Int32 GetHashCode()
        {
            return Entity.Id.GetHashCode();
        }
    }
}
