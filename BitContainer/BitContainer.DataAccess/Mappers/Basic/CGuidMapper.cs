using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace BitContainer.DataAccess.Mappers
{
    public class CGuidMapper : IMapper<Guid>
    {
        public Guid ReadItem(SqlDataReader rd)
        {
            return rd.GetGuid(0);
        }
    }
}
