using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models.StorageEntities;

namespace BitContainer.DataAccess.Mappers.StorageEntities
{
    public class CFileStreamInfoMapper : IMapper<CFileStreamInfo>
    {
        public CFileStreamInfo ReadItem(SqlDataReader rd)
        {
            String path = rd.GetString("Path");
            Byte[] context = (Byte[])rd["TransactionContext"];
            
            return new CFileStreamInfo()
            {
                Path = path,
                TransactionContext = context
            };
        }
    }
}
