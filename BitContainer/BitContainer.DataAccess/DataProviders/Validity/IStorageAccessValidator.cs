using BitContainer.DataAccess.DataProviders.Storage;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.DataProviders.Validity
{
    public interface IStorageAccessValidator
    {
        IEntityValidityChecker EntityExists(CStorageEntityId entityId);
        void EntityNotExists(CStorageEntityId parentId, CUserId userId, string name);
    }
}
