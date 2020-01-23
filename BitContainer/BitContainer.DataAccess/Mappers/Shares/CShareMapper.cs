using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Mappers.Shares
{
    public class CShareMapper : IMapper<CShare>
    {
        public CShare ReadItem(SqlDataReader rd)
        {
            CUserId userId = rd.GetUserId(DbNames.Shares.UserApprovedId);
            CStorageEntityId entityId = rd.GetStorageEntityId(DbNames.Shares.EntityId);
            EAccessType accessType = rd.GetAccessType(DbNames.Shares.AccessTypeId);

            return new CShare(userId, entityId, accessType);
        }
    }
}
