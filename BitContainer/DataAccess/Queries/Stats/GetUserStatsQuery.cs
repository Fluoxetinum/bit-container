using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Stats
{
    public class GetUserStatsQuery : AbstractScalarQuery<CUserStats>
    {
        public Guid UserId { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Stats.Id}, {DbNames.Stats.UserId}, " +
            $"{DbNames.Stats.FilesCount}, {DbNames.Stats.DirsCount}, {DbNames.Stats.StorageSize} " +
            $"FROM {DbNames.Stats} " +
            $"WHERE {DbNames.Stats.UserId} = @{nameof(UserId)}";

        public GetUserStatsQuery(Guid userId) : base(new CUserStatsMapper())
        {
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(UserId), UserId);
            return command;
        }
    }
}
