using System.Collections.Generic;
using BitContainer.DataAccess.DataProviders.Storage;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Shares;
using BitContainer.DataAccess.Queries.StorageEntites;
using BitContainer.Shared.Models;
using Microsoft.SqlServer.Management.Common;

namespace BitContainer.DataAccess.DataProviders.Validity
{
    public class CStorageAccessValidator : IStorageAccessValidator
    {
        private readonly ISqlDbHelper _dbHelper;

        public CStorageAccessValidator(ISqlDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IEntityValidityChecker EntityExists(CStorageEntityId entityId)
        {
            if (entityId.IsRootId)
                return new CEntityValidityChecker(CDirectory.Root, new List<CShare>());

            var getEntityQuery = new GetStorageEntityQuery(entityId);
            IStorageEntity entity = _dbHelper.ExecuteQuery(getEntityQuery);

            if (entity == null)
                throw new InvalidArgumentException("No such entity.");

            var query = new GetEntitySharesQuery(entity.Id);
            List<CShare> shares = _dbHelper.ExecuteQuery(query);

            return new CEntityValidityChecker(entity, shares);
        }

        public void EntityNotExists(CStorageEntityId parentId, CUserId userId, string name)
        {
            var getEntityQuery = new GetStorageEntityQuery(parentId, userId, name);
            IStorageEntity entity = _dbHelper.ExecuteQuery(getEntityQuery);

            if (entity != null)
                throw new InvalidArgumentException("Such entity already exists.");
        }
    }
}
