using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.StorageEntites
{
    class DeleteEntityQuery : AbstractWriteQuery
    {
        public CStorageEntityId EntityId { get; set; }

        private static readonly String QueryString = 
            $"DELETE FROM {DbNames.Shares} WHERE {DbNames.Shares.PxEntityId} = @{nameof(EntityId)};" +
            $"DELETE FROM {DbNames.Entities} WHERE {DbNames.Entities.PxId} = @{nameof(EntityId)};";

        public DeleteEntityQuery(CStorageEntityId entityId)
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
