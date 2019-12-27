using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries;
using BitContainer.DataAccess.Queries.Add;
using BitContainer.DataAccess.Queries.Get;
using BitContainer.DataAccess.Queries.Share;
using BitContainer.DataAccess.Queries.Store;

namespace BitContainer.DataAccess.DataProviders
{
    public class CSharesProvider : ISharesProvider
    {
        public ERestrictedAccessType CheckStorageEntityAccess(Guid entityId, Guid userId)
        {
            if (entityId.IsRootDir()) return ERestrictedAccessType.Write;

            var ownerQuery = new GetOwnerQuery(entityId);
            CUser owner = CDbHelper.ExecuteQuery(ownerQuery);
            var shareQuery = new GetShareByIdQuery(entityId, userId);
            CShare share = CDbHelper.ExecuteQuery(shareQuery);

            return ComputeAccess(userId, owner, share);
        }

        private ERestrictedAccessType ComputeAccess(Guid subjectUserId, CUser owner, CShare share)
        {
            if (subjectUserId == owner.Id) return ERestrictedAccessType.Write;
            if (share != null) return share.RestrictedAccessType;
            return ERestrictedAccessType.None;
        }

        public CUser GetStorageEntityOwner(Guid entityId)
        {
            var query = new GetOwnerQuery(entityId);
            return CDbHelper.ExecuteQuery(query);
        }
        
        public Boolean IsStorageEntityHasShare(Guid entityId)
        {
            var query = new IsEntityHasShare(entityId);
            return CDbHelper.ExecuteQuery(query);
        }

        public Int32 AddStorageEntityShare(Guid personId, ERestrictedAccessType type, Guid entityId)
        {
            var query = new AddShareQuery(personId, type, entityId);
            return CDbHelper.ExecuteQuery(query);
        }

        public Int32 UpdateStorageEntityShare(Guid personId, ERestrictedAccessType type, Guid entityId)
        {
            var query = new UpdateShareQuery(personId, type, entityId);
            return CDbHelper.ExecuteQuery(query);
        }

        public Int32 DeleteStorgeEntityShare(Guid personId, Guid entityId)
        {
            var query = new RemoveShareQuery(entityId, personId);
            return CDbHelper.ExecuteQuery(query);
        }

        public CShare GetStorageEntityShare(Guid personId, Guid entityId)
        {
            var query = new GetShareByIdQuery(entityId, personId);
            return CDbHelper.ExecuteQuery(query);
        }
    }
}
