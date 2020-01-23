using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Stats
{
    public class UpdateStatsQuery : AbstractWriteQuery
    {
        public CUserId UserId { get; set; }
        public Int32 FilesCount { get; set; }
        public Int32 DirsCount { get; set; }
        public Int64 StorageSize { get; set; }

        private static readonly String _queryString = 
            $"UPDATE {DbNames.Stats} " +
            $"SET {DbNames.Stats.PxFilesCount} = @{nameof(FilesCount)}, " +
            $"{DbNames.Stats.PxDirsCount} = @{nameof(DirsCount)}, " +
            $"{DbNames.Stats.PxStorageSize} = @{nameof(StorageSize)} " +
            $"WHERE {DbNames.Stats.PxUserId} = @{nameof(UserId)}; ";

        public UpdateStatsQuery(CStats stats)
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
            command.Parameters.AddWithValue(nameof(UserId), UserId.ToGuid());
            return command;
        }
    }
}
