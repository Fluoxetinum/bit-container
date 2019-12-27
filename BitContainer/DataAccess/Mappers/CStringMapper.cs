using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Text;

namespace BitContainer.DataAccess.Mappers
{
    class CStringMapper : IMapper<String>
    {
        public string ReadItem(SqlDataReader rd)
        {
            return rd.GetString(0);
        }
    }
}
