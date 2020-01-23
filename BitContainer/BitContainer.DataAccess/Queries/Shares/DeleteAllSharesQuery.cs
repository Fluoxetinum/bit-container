using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Shares
{
    public class DeleteAllSharesQuery : AbstractWriteQuery
    {
        public CStorageEntityId EntityId { get; set; }

        private static readonly String QueryString = $"DELETE FROM {DbNames.Shares} " +
                                                     $"WHERE {DbNames.Shares.PxEntityId} = @{nameof(EntityId)}";

        public DeleteAllSharesQuery(CStorageEntityId entityId)
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
