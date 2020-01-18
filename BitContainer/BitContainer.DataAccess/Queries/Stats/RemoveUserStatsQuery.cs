using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Stats
{
    public class RemoveUserStatsQuery: AbstractWriteQuery
    {
        public Guid UserId { get; set; }

        private String _queryString = $"DELETE FROM {DbNames.Stats} " +
                                      $"WHERE {DbNames.Stats.UserId} = @{nameof(UserId)};";

        public RemoveUserStatsQuery(Guid userId)
        {
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = _queryString;
            command.Parameters.AddWithValue(nameof(UserId), UserId);
            return command;
        }
    }
}
