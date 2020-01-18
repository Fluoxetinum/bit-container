using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Stats
{
    public class AddNewUserStatsQuery : AbstractWriteQuery
    {
        public Guid UserId { get; set; }

        private static readonly String Query = 
            $"INSERT INTO {DbNames.Stats} " +
            $"({DbNames.Stats.UserId}, {DbNames.Stats.FilesCount}, " +
            $"{DbNames.Stats.DirsCount}, {DbNames.Stats.StorageSize}) " +
            $"VALUES (@{nameof(UserId)}, 0, 0, 0)";

        public AddNewUserStatsQuery(Guid userId)
        {
            UserId = userId;
        }
        
        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = Query;
            command.Parameters.AddWithValue(nameof(UserId), UserId);
            return command;
        }
    }
}
