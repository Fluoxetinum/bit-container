using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.Mappers
{
    public class CUserStatsMapper : IMapper<CUserStats>
    {
        public CUserStats ReadItem(SqlDataReader rd)
        {
            Guid userId = rd.GetGuid("UserID");
            Int32 filesCount = rd.GetInt32("FilesCount");
            Int32 dirsCount = rd.GetInt32("DirectoriesCount");
            Int32 storageSize = rd.GetInt32("StorageSize");

            return new CUserStats()
            {
                UserId = userId,
                FilesCount = filesCount,
                DirsCount = dirsCount,
                StorageSize = storageSize
            };
        }
    }
}
