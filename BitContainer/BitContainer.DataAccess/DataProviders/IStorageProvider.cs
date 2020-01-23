using BitContainer.DataAccess.DataProviders.Storage;
using BitContainer.DataAccess.DataProviders.Validity;

namespace BitContainer.DataAccess.DataProviders.Interfaces
{
    public interface IStorageProvider
    {
        IStorageEntitiesProvider Entities { get; }
        IStorageSharesProvider Shares { get; }
        IStorageAccessValidator Validator { get; }
        IStatsProvider Stats { get; }
    }
}
