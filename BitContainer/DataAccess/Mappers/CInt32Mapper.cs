using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace BitContainer.DataAccess.Mappers
{
    class CInt32Mapper : IMapper<Int32>
    {
        public int ReadItem(SqlDataReader rd)
        {
            return rd.GetInt32(0);
        }
    }
}
