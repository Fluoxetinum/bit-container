using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Stats
{
    public class UpdateUserStatsQuery : AbstractWriteQuery
    {
        public Guid UserId { get; set; }
        public Int32 FilesCount { get; set; }
        public Int32 DirsCount { get; set; }
        public Int32 StorageSize { get; set; }

        private static readonly String _queryString = 
            $"UPDATE {DbNames.Stats} " +
            $"SET {DbNames.Stats.FilesCount} = @{nameof(FilesCount)}, " +
            $"{DbNames.Stats.DirsCount} = @{nameof(DirsCount)}, " +
            $"{DbNames.Stats.StorageSize} = @{nameof(StorageSize)} " +
            $"WHERE {DbNames.Stats.UserId} = @{nameof(UserId)}; ";

        public UpdateUserStatsQuery(CUserStats stats)
        {
            UserId = stats.UserId;
            FilesCount = stats.FilesCount;
            DirsCount = stats.DirsCount;
            StorageSize = stats.StorageSize;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = _queryString;
            command.Parameters.AddWithValue(nameof(FilesCount), FilesCount);
            command.Parameters.AddWithValue(nameof(DirsCount), DirsCount);
            command.Parameters.AddWithValue(nameof(StorageSize), StorageSize);
            command.Parameters.AddWithValue(nameof(UserId), UserId);
            return command;
        }
    }
}
