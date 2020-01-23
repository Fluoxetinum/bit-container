using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.DataProviders.Validity
{
    public interface IEntityValidityChecker
    {
        IEntityValidityChecker IsNotRoot();
        IEntityValidityChecker IsDir();
        IEntityValidityChecker IsOwner(CUserId userId);
        IEntityValidityChecker HasReadAccess(CUserId userId);
        IEntityValidityChecker HasWriteAccess(CUserId userId);
        CSharableEntity ToSharableEntity(CUserId userId);
        IStorageEntity ToEntity();
    }
}
