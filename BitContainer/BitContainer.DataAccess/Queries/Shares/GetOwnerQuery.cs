using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Mappers.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Shares
{
    public class GetOwnerQuery : AbstractScalarQuery<CUserId>
    {
        public CStorageEntityId EntityId { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Entities.PxOwnerId} " +
            $"FROM {DbNames.Entities} " +
            $"WHERE {DbNames.Entities.PxId} = @{nameof(EntityId)};";

        public GetOwnerQuery(CStorageEntityId entityId) : base(new CUserIdMapper())
        {
            EntityId = entityId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(EntityId), EntityId.ToGuid());
            return command;
        }
    }
}