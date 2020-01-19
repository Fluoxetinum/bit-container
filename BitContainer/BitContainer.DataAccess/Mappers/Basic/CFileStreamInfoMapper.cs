using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.Mappers.Basic
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
