using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Threading.Tasks;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.DataProviders.Storage
{
    public interface IStorageEntitiesProvider
    {
        List<CSharableEntity> GetOwnerChildren(CStorageEntityId parentId, CUserId userId);
        List<CSharableEntity> GetSharedChildren(CStorageEntityId parentId, CUserId userId);

        List<CSearchResult> SearchOwnByName(string pattern, CStorageEntityId parentId, CUserId userId);
        List<CSearchResult> SearchSharedByName(string pattern, CStorageEntityId parentId, CUserId userId);

        SortedDictionary<Int32, List<IStorageEntity>> GetChildrenAsc(CStorageEntityId entityId);
        IStorageEntity GetStorageEntity(CStorageEntityId entityId);
        IStorageEntity GetStorageEntity(CStorageEntityId parentId, CUserId userId, string name);
        Boolean EntityExists(CStorageEntityId parentId, CUserId userId, string name);
        void DeleteEntity(CStorageEntityId entityId);
        void RenameEntity(CStorageEntityId entityId, string name);
        Task CopyEntity(CStorageEntityId entityId, CUserId userId, IStorageEntity newParent);

        Task<CSharableEntity> AddFileAsync(Stream sourceStream, IStorageEntity parent, CUserId ownerId, String name,
            Int64 size);
        SqlFileStream GetFileStream(SqlCommand command, CStorageEntityId fileId, FileAccess fileAccess);
        void UpdateFileSize(SqlCommand command, CStorageEntityId fileId, Int64 newSize);

        CSharableEntity AddDir(IStorageEntity parent, CUserId ownerId, string name);
    }
}
