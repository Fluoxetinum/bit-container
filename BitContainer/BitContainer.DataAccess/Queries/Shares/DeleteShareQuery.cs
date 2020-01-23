using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Shares
{
    public class DeleteShareQuery : AbstractWriteQuery
    {
        public CStorageEntityId EntityId { get; set; }
        public CUserId UserId { get; set; }

        private static readonly String QueryString = 
            $"DELETE FROM {DbNames.Shares} " +
            $"WHERE {DbNames.Shares.PxEntityId} = @{nameof(EntityId)} " +
            $"AND {DbNames.Shares.PxUserApprovedId} = @{nameof(UserId)};";

        public DeleteShareQuery(CStorageEntityId entityId, CUserId userId)
        {
            EntityId = entityId;
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(EntityId), EntityId.ToGuid());
            command.Parameters.AddWithValue(nameof(UserId), UserId.ToGuid());
            return command;
        }
    }
}
