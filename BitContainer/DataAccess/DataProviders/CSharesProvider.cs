using System;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Add;
using BitContainer.DataAccess.Queries.Share;

namespace BitContainer.DataAccess.DataProviders
{
    public class CSharesProvider : ISharesProvider
    {
        private readonly ISqlDbHelper _dbHelper;

        public CSharesProvider(ISqlDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public ERestrictedAccessType CheckStorageEntityAccess(Guid entityId, Guid userId)
        {
            if (entityId.IsRootDir()) return ERestrictedAccessType.Write;

            var ownerQuery = new GetOwnerQuery(entityId);
            Guid ownerId = _dbHelper.ExecuteQuery(ownerQuery);
            var shareQuery = new GetShareByIdQuery(entityId, userId);
            CShare share = _dbHelper.ExecuteQuery(shareQuery);

            return ComputeAccess(userId, ownerId, share);
        }

        private ERestrictedAccessType ComputeAccess(Guid subjectUserId, Guid ownerId, CShare share)
        {
            if (subjectUserId == ownerId) return ERestrictedAccessType.Write;
            if (share != null) return share.RestrictedAccessType;
            return ERestrictedAccessType.None;
        }

        public Guid GetStorageEntityOwner(Guid entityId)
        {
            var query = new GetOwnerQuery(entityId);
            return _dbHelper.ExecuteQuery(query);
        }
        
        public Boolean IsStorageEntityHasShare(Guid entityId)
        {
            var query = new IsEntityHasShare(entityId);
            return _dbHelper.ExecuteQuery(query);
        }

        public Int32 AddStorageEntityShare(Guid personId, ERestrictedAccessType type, Guid entityId)
        {
            var query = new AddShareQuery(personId, type, entityId);
            return _dbHelper.ExecuteQuery(query);
        }

        public Int32 UpdateStorageEntityShare(Guid personId, ERestrictedAccessType type, Guid entityId)
        {
            var query = new UpdateShareQuery(personId, type, entityId);
            return _dbHelper.ExecuteQuery(query);
        }

        public Int32 DeleteStorgeEntityShare(Guid personId, Guid entityId)
        {
            var query = new RemoveShareQuery(entityId, personId);
            return _dbHelper.ExecuteQuery(query);
        }

        public CShare GetStorageEntityShare(Guid personId, Guid entityId)
        {
            var query = new GetShareByIdQuery(entityId, personId);
            return _dbHelper.ExecuteQuery(query);
        }
    }
}
