using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.DataProviders.Interfaces
{
    public interface IStorageProvider
    {
        IStorageEntitiesProvider StorageEntities { get; }
        ISharesProvider Shares { get; }

        List<CSearchResult> SearchOwnEntitiesByName(String pattern, Guid parentId, Guid ownerId);
        List<CSearchResult> SearchRestrictedEntitiesByName(String pattern, Guid parentId, Guid ownerId);
    }
}
