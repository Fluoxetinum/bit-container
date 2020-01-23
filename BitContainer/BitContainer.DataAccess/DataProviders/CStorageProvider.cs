using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.DataProviders.Storage;
using BitContainer.DataAccess.DataProviders.Validity;

namespace BitContainer.DataAccess.DataProviders
{
    public class CStorageProvider : IStorageProvider
    {
        public IStorageEntitiesProvider Entities { get;  }
        public IStorageSharesProvider Shares { get; }
        public IStorageAccessValidator Validator { get; }
        public IStatsProvider Stats { get; }

        public CStorageProvider(ISqlDbHelper sqlDbHelper)
        {
            Stats = new CStatsProvider(sqlDbHelper);
            Shares = new CStorageSharesProvider(sqlDbHelper);
            Entities = new CStorageEntitiesProvider(sqlDbHelper, Stats, Shares);
            Validator = new CStorageAccessValidator(sqlDbHelper);
        }
    }
}
