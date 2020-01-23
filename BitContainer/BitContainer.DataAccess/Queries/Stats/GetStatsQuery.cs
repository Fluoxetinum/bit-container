using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers.StorageEntities;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Stats
{
    public class GetStatsQuery : AbstractScalarQuery<CStats>
    {
        public CUserId UserId { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Stats.PxUserId}, " +
            $"{DbNames.Stats.PxFilesCount}, {DbNames.Stats.PxDirsCount}, {DbNames.Stats.PxStorageSize} " +
            $"FROM {DbNames.Stats} " +
            $"WHERE {DbNames.Stats.PxUserId} = @{nameof(UserId)}";

        public GetStatsQuery(CUserId userId) : base(new CStatsMapper())
        {
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(UserId), UserId.ToGuid());
            return command;
        }
    }
}
