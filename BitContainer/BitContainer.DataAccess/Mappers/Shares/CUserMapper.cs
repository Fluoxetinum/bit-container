using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Mappers.Shares
{
    class CUserMapper : IMapper<CUser>
    {
        public CUser ReadItem(SqlDataReader rd)
        {
            CUserId id = rd.GetUserId(DbNames.Users.Id);
            String name = rd.GetString(DbNames.Users.Name);
            return new CUser(id, name);
        }
    }
}
