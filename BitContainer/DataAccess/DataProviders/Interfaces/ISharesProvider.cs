using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.DataProviders.Interfaces
{
    public interface ISharesProvider
    {
        ERestrictedAccessType CheckStorageEntityAccess(Guid entityId, Guid userId);

        CUser GetStorageEntityOwner(Guid entityId);

        Boolean IsStorageEntityHasShare(Guid entityId);

        Int32 AddStorageEntityShare(Guid personId, ERestrictedAccessType type, Guid entityId);
        Int32 UpdateStorageEntityShare(Guid personId, ERestrictedAccessType type, Guid entityId);
        Int32 DeleteStorgeEntityShare(Guid personId, Guid entityId);

        CShare GetStorageEntityShare(Guid personId, Guid entityId);
    }
}
