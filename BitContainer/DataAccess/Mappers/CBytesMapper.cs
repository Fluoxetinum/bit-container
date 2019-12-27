using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace BitContainer.DataAccess.Mappers
{
    class CBytesMapper : IMapper<Byte[]>
    {
        public byte[] ReadItem(SqlDataReader rd)
        {
            return Convert.FromBase64String(rd.GetString(0));
        }
    }
}
