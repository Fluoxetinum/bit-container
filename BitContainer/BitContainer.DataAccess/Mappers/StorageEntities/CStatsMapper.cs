using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Mappers.StorageEntities
{
    public class CStatsMapper : IMapper<CStats>
    {
        public CStats ReadItem(SqlDataReader rd)
        {
            CUserId userId = rd.GetUserId(DbNames.Stats.UserId);
            Int32 filesCount = rd.GetInt32(DbNames.Stats.FilesCount);
            Int32 dirsCount = rd.GetInt32(DbNames.Stats.DirsCount);
            Int64 storageSize = rd.GetInt64(DbNames.Stats.StorageSize);

            return new CStats(userId, filesCount, dirsCount, storageSize);
        }
    }
}
