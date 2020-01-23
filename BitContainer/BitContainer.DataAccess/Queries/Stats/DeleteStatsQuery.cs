using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Stats
{
    public class DeleteStatsQuery: AbstractWriteQuery
    {
        public CUserId UserId { get; set; }

        private readonly String _queryString = $"DELETE FROM {DbNames.Stats} " +
                                      $"WHERE {DbNames.Stats.PxUserId} = @{nameof(UserId)};";

        public DeleteStatsQuery(CUserId userId)
        {
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = _queryString;
            command.Parameters.AddWithValue(nameof(UserId), UserId.ToGuid());
            return command;
        }
    }
}
