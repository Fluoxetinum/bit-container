using BitContainer.DataAccess.Helpers;
using System;
using System.Data.Sql;
using System.Data.SqlClient;

namespace BitContainer.DataAccess.Mappers
{
    class CFileDataMapper : IMapper<Byte[]>
    {
        public Byte[] ReadItem(SqlDataReader rd)
        {
            Int32 size = rd.GetInt32("Size");
            return rd.GetBytes("Data", size);
        }
    }
}
