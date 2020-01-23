using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Authentication;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Shares;
using BitContainer.DataAccess.Queries.StorageEntites;
using BitContainer.Shared.Models;
using Microsoft.SqlServer.Management.Common;

namespace BitContainer.DataAccess.DataProviders.Storage
{
    public class CStorageSharesProvider : IStorageSharesProvider
    {
        private readonly ISqlDbHelper _dbHelper;

        public CStorageSharesProvider(ISqlDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        private void SaveWithChildrenShares(SqlCommand command, CStorageEntityId entityId, CUserId userId, EAccessType type)
        {
            var getEntityQuery = new GetStorageEntityQuery(entityId);
            IStorageEntity entity = getEntityQuery.Execute(command);

            var userSharesQuery = new GetUserSharesQuery(userId);
            Dictionary<CStorageEntityId, EAccessType> shares = userSharesQuery.Execute(command);

            if (shares.ContainsKey(entity.ParentId) && shares[entity.ParentId] > type)
                throw new InvalidOperationException("Lowering access on shared folder's child is not allowed.");

            var childrenQuery = new GetChildrenQuery(entityId);
            SortedDictionary<Int32, List<IStorageEntity>> children = childrenQuery.Execute(command);

            UpsertShare(command, entityId, userId, type, shares);

            foreach (var level in children)
            {
                var childrenOnLevel = level.Value;
                foreach (var child in childrenOnLevel)
                {
                    UpsertShare(command, child.Id, userId, type, shares);
                }
            }
        }

        private void UpsertShare(SqlCommand command, CStorageEntityId entityId, CUserId userId, EAccessType type,
            Dictionary<CStorageEntityId, EAccessType> shares)
        {
            if (shares.ContainsKey(entityId))
                UpdateShare(command, entityId, userId, type);
            else
                AddShare(command, entityId, userId, type);
        }

        private void AddShare(SqlCommand command, CStorageEntityId entityId, CUserId userId, EAccessType type)
        {
            var query = new AddShareQuery(userId, type, entityId);
            query.Execute(command);
        }

        private void UpdateShare(SqlCommand command, CStorageEntityId entityId, CUserId userId, EAccessType type)
        {
            var query = new UpdateShareQuery(userId, type, entityId);
            query.Execute(command);
        }

        private void DeleteWithChildrenShares(SqlCommand command, CStorageEntityId entityId, CUserId userId)
        {
            var getEntityQuery = new GetStorageEntityQuery(entityId);
            IStorageEntity entity = getEntityQuery.Execute(command);

            var userSharesQuery = new GetUserSharesQuery(userId);
            Dictionary<CStorageEntityId, EAccessType> shares = userSharesQuery.Execute(command);

            if (shares.ContainsKey(entity.ParentId))
                throw new InvalidOperationException("Deleting shares is allowed for root shared directories only.");

            var childrenQuery = new GetChildrenQuery(entityId);
            SortedDictionary<Int32, List<IStorageEntity>> children = childrenQuery.Execute(command);

            DeleteShare(command, entityId, userId);

            foreach (var level in children)
            {
                var childrenOnLevel = level.Value;
                foreach (var child in childrenOnLevel)
                {
                    DeleteShare(command, child.Id, userId);
                }
            }
        }

        private void DeleteShare(SqlCommand command, CStorageEntityId entityId, CUserId userId)
        {
            var query = new DeleteShareQuery(entityId, userId);
            query.Execute(command);
        }

        public void SaveShare(CStorageEntityId entityId, CUserId userId, EAccessType type)
        {
            entityId.IfRootId(() => throw new InvalidArgumentException("Sharing root is not allowed."));

            _dbHelper.ExecuteTransaction((command) => SaveWithChildrenShares(command, entityId, userId, type));
        }

        public void DeleteShare(CStorageEntityId entityId, CUserId userId)
        {
            entityId.IfRootId(() => throw new InvalidArgumentException("Sharing root is not allowed."));

            _dbHelper.ExecuteTransaction((command) => DeleteWithChildrenShares(command, entityId, userId));
        }

        public List<CShare> GetShares(CStorageEntityId entityId)
        {
            var query = new GetEntitySharesQuery(entityId);
            return _dbHelper.ExecuteQuery(query);
        }

        public List<CShare> CopyShares(SqlCommand command, CStorageEntityId entitySourceId, CStorageEntityId entityDestId, CUserId userId)
        {
            var queryShares = new GetEntitySharesQuery(entitySourceId);
            List<CShare> shares = queryShares.Execute(command);

            foreach (var share in shares)
            {
                if (share.UserId != userId)
                    AddShare(command, entityDestId, share.UserId, share.AccessType);
                else
                    AddShare(command, entityDestId, share.UserId, EAccessType.Write);
            }

            return shares;
        }

        public void DeleteShares(SqlCommand command, CStorageEntityId entityId)
        {
            var query = new DeleteAllSharesQuery(entityId);
            query.Execute(command);
        }
    }
}
