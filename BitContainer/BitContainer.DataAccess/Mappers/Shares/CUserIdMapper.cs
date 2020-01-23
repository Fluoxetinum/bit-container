using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Mappers.Shares
{
    public class CUserIdMapper : IMapper<CUserId>
    {
        public CUserId ReadItem(SqlDataReader rd)
        {
            return rd.GetUserId(DbNames.Users.Id);
        }
    }
}
