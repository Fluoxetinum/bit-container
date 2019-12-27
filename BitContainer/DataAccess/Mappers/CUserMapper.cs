using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.Mappers
{
    class CUserMapper : IMapper<CUser>
    {
        public CUser ReadItem(SqlDataReader rd)
        {
            Guid id = rd.GetGuid("ID");
            String name = rd.GetString("Name");
            return CUser.Create(id, name);
        }
    }
}
