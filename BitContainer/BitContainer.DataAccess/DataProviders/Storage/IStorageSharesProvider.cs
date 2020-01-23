using System.Collections.Generic;
using System.Data.SqlClient;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.DataProviders.Storage
{
    public interface IStorageSharesProvider
    {
        void SaveShare(CStorageEntityId entityId, CUserId userId, EAccessType type);
        void DeleteShare(CStorageEntityId entityId, CUserId userId);
        List<CShare> GetShares(CStorageEntityId entityId);

        List<CShare> CopyShares(SqlCommand command, CStorageEntityId entitySourceId, CStorageEntityId entityDestId,
            CUserId userId);
        void DeleteShares(SqlCommand command, CStorageEntityId entityId);
    }
}
