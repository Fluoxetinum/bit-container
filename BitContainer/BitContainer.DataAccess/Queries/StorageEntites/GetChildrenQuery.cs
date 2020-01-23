using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Mappers.StorageEntities;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.StorageEntites
{
    public class GetChildrenQuery : AbstractSortedGroupQuery<Int32, IStorageEntity>
    {
        public CStorageEntityId EntityId { get; set; }

        private static readonly String QueryString =
                                $"WITH SEReq AS " +
                                $" ( " +
                                $"  SELECT {DbNames.Entities.PxId}, {DbNames.Entities.PxParentId}, {DbNames.Entities.PxOwnerId}, " +
                                $"      {DbNames.Entities.PxName}, {DbNames.Entities.PxCreated}, {DbNames.Entities.PxSize}, 1 AS Level " +
                                $"  FROM {DbNames.Entities} " +
                                $"  WHERE {DbNames.Entities.PxId} = @{nameof(EntityId)} " +
                                $"  UNION ALL " +
                                $"  SELECT SE.ID, SE.ParentID, SE.OwnerID, SE.Name, SE.Created, SE.Size, SEReq.Level+1 AS Level " +
                                $"  FROM {DbNames.Entities} AS SE " +
                                $"  INNER JOIN SEReq " +
                                $"    ON SEReq.ID = SE.ParentID " +
                                $" ) " +
                                $" SELECT Level, {DbNames.Entities.PxId}, {DbNames.Entities.PxParentId}, {DbNames.Entities.PxOwnerId}, " +
                                $"      {DbNames.Entities.PxName}, {DbNames.Entities.PxCreated}, {DbNames.Entities.PxSize} " +
                                $" FROM SEReq AS {DbNames.Entities} " +
                                $" WHERE Level != 1; ";

        public GetChildrenQuery(CStorageEntityId entityId) 
            : base(new CInt32Mapper(), new CStorageEntityMapper())
        {
            EntityId = entityId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(EntityId), EntityId.ToGuid());
            return command;
        }

        public GetChildrenQuery Ascending()
        {
            Comparer = Comparer<Int32>.Default;
            return this;
        }

        public GetChildrenQuery Descending()
        {
            Comparer = Comparer<Int32>.Create(((x, y) => y.CompareTo(x)));
            return this;
        }
    }
}
