using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace BitContainer.DataAccess.Mappers
{
    public class CBooleanMapper : IMapper<Boolean>
    {
        public bool ReadItem(SqlDataReader rd)
        {
            return rd.GetInt32(0) > 0;
        }
    }
}
