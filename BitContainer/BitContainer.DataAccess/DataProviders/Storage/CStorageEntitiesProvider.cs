using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Shares;
using BitContainer.DataAccess.Queries.StorageEntites;
using BitContainer.DataAccess.Queries.Store;
using BitContainer.Shared.Models;
using BitContainer.Shared.StreamHelpers;

namespace BitContainer.DataAccess.DataProviders.Storage
{
    public class CStorageEntitiesProvider : IStorageEntitiesProvider
    {
        private readonly ISqlDbHelper _dbHelper;
        private readonly IStatsProvider _statsProvider;
        private readonly IStorageSharesProvider _sharesProvider;
        
        public CStorageEntitiesProvider(ISqlDbHelper dbHelper, 
            IStatsProvider statsProvider, 
            IStorageSharesProvider sharesProvider)
        {
            _dbHelper = dbHelper;
            _statsProvider = statsProvider;
            _sharesProvider = sharesProvider;
        }

        private void DeleteWithChildrenEntities(SqlCommand command, CStorageEntityId entityId)
        {
            IStorageEntity entity = GetStorageEntity(command, entityId);
            CStats stats = _statsProvider.GetStats(command, entity.OwnerId);

            var childrenQuery = new GetChildrenQuery(entityId).Descending();
            SortedDictionary<Int32, List<IStorageEntity>> children = childrenQuery.Execute(command);
            
            _sharesProvider.DeleteShares(command, entity.Id);

            foreach (var level in children)
            {
                foreach (var child in level.Value)
                {
                    DeleteEntity(command, child.Id);
                    stats.DeleteUpdate(child);
                }
            }
            
            DeleteEntity(command, entity.Id);
            stats.DeleteUpdate(entity);

            _statsProvider.UpdateStats(command, stats);
        }

        private void DeleteEntity(SqlCommand command, CStorageEntityId entityId)
        {
            var query = new DeleteEntityQuery(entityId);
            query.Execute(command);
        }

        private IStorageEntity GetStorageEntity(SqlCommand command, CStorageEntityId entityId)
        {
            var query = new GetStorageEntityQuery(entityId);
            return query.Execute(command);
        }
        
        private async Task<CStorageEntityId> CopyDir(SqlCommand command, CDirectory dirToCopy, CUserId userId, IStorageEntity newParentId)
        {
            var childrenQuery = new GetChildrenQuery(dirToCopy.Id).Ascending();
            SortedDictionary<Int32, List<IStorageEntity>> children = childrenQuery.Execute(command);

            var newEntites = new Dictionary<CStorageEntityId, IStorageEntity>();

            CSharableEntity newRootDir = AddDir(command, newParentId, userId, dirToCopy.Name);
            newEntites[dirToCopy.Id] = newRootDir.AccessWrapper.Entity;

            foreach (var level in children)
            {
                foreach (var child in level.Value)
                {
                    switch (child)
                    {
                        case CFile file:
                            await CopyFile(command, file, userId, newEntites[file.ParentId]);
                            break;

                        case CDirectory dir:
                            CSharableEntity newDir = AddDir(command, newEntites[dir.ParentId], userId, dir.Name);
                            newEntites[dir.Id] = newDir.AccessWrapper.Entity;
                            break;
                        default:
                            throw new InvalidCastException(nameof(child));
                    }
                }
            }

            return newRootDir.AccessWrapper.Entity.Id;
        }

        private async Task<CStorageEntityId> CopyFile(SqlCommand command, CFile file, CUserId userId, IStorageEntity newParentId)
        {
            await using SqlFileStream fileStream = GetFileStream(command, file.Id, FileAccess.Read);
            CSharableEntity entity = await AddFileAsync(command, sourceStream: fileStream, newParentId, userId, file.Name, file.Size);
            return entity.AccessWrapper.Entity.Id;
        }

        private CFileStreamInfo GetFileStreamInfo(SqlCommand command, CStorageEntityId fileId)
        {
            var query = new GetFileStreamPathQuery(fileId);
            return query.Execute(command);
        }

        private CSharableEntity AddEmptyFile(SqlCommand command, IStorageEntity parent, CUserId userId, String name)
        {
            CStorageEntityId parentId = parent.Id;
            CUserId parentOwner = parentId.IsRootId ? userId : parent.OwnerId;

            CStats stats = _statsProvider.GetStats(command, parentOwner);
            
            byte[] mockFileData = new byte[0];
            var query = new AddEntityQuery(parentId, parentOwner, name, mockFileData);
            query.Execute(command);

            var fileQuery = new GetStorageEntityQuery(parentId, parentOwner, name);
            IStorageEntity file = fileQuery.Execute(command);

            List<CShare> shares = 
                _sharesProvider.CopyShares(command, parentId, file.Id, userId);

            stats.AddUpdate(file);
            _statsProvider.UpdateStats(command, stats);
            
            CAccessWrapper accessWrapper = new CAccessWrapper(file, EAccessType.Write);
            CSharableEntity sharableEntity = new CSharableEntity(accessWrapper, shares);

            return sharableEntity;
        }

        private CSharableEntity AddDir(SqlCommand command, IStorageEntity parent, CUserId userId, String name)
        {
            CStorageEntityId parentId = parent.Id;
            CUserId parentOwner = parentId.IsRootId ? userId : parent.OwnerId;

            CStats stats = _statsProvider.GetStats(command, parentOwner);
            
            var query = new AddEntityQuery(parentId, parentOwner, name);
            query.Execute(command);

            var getQuery = new GetStorageEntityQuery(parentId, parentOwner, name);
            IStorageEntity dir = getQuery.Execute(command);

            List<CShare> shares = 
                _sharesProvider.CopyShares(command, parentId, dir.Id, userId);

            stats.AddUpdate(dir);
            _statsProvider.UpdateStats(command, stats);

            CAccessWrapper accessWrapper = new CAccessWrapper(dir, EAccessType.Write);
            CSharableEntity sharableEntity = new CSharableEntity(accessWrapper, shares);

            return sharableEntity;
        }

        private List<CSearchResult> SearchByName(String pattern, CStorageEntityId searchParentId, CUserId userId,
            Func<CStorageEntityId, CUserId, List<CSharableEntity>> childrenSearcher)
        {
            var searchResult = new List<CSearchResult>(); 

            var allChecked = new Dictionary<CStorageEntityId, CAccessWrapper>();
            var found = new List<CSharableEntity>();
            var searchStack = new Stack<CSharableEntity>();

            foreach (var children in childrenSearcher(searchParentId, userId))
                searchStack.Push(children);

            while (searchStack.Count > 0)
            {
                var child = searchStack.Pop();

                CStorageEntityId currentEntityId = child.AccessWrapper.Entity.Id;
                String currentEntityName = child.AccessWrapper.Entity.Name;

                if (Regex.IsMatch(currentEntityName, $".*{pattern}.*", RegexOptions.IgnoreCase))
                    found.Add(child);
                
                allChecked[currentEntityId] = child.AccessWrapper;

                foreach (var children in childrenSearcher(currentEntityId, userId))
                    searchStack.Push(children);
            }

            foreach (var r in found)
            {
                var parents = new LinkedList<CStorageEntityId>();
                parents.AddFirst(r.AccessWrapper.Entity.Id);

                CStorageEntityId currentParentId = r.AccessWrapper.Entity.ParentId;
                while (currentParentId != searchParentId)
                {
                    CAccessWrapper currentParent = allChecked[currentParentId];
                    parents.AddFirst(currentParentId);
                    currentParentId = currentParent.Entity.ParentId;
                }
                parents.AddFirst(searchParentId);

                searchResult.Add(new CSearchResult(r, parents));
            }

            return searchResult;
        }
        
        private async Task<CSharableEntity> AddFileAsync(SqlCommand command, Stream sourceStream, IStorageEntity parent, CUserId userId,
            String name, Int64 size)
        {
            CSharableEntity file = AddEmptyFile(command, parent, userId, name);
            CStorageEntityId fileId = file.AccessWrapper.Entity.Id;
            
            await using (SqlFileStream fileStream = GetFileStream(command, fileId, FileAccess.Write))
            {
                await StreamEx.WriteToStream(fromStream:sourceStream, toStream:fileStream, size);
            }
                
            if (sourceStream is SqlFileStream) sourceStream.Dispose();

            UpdateFileSize(command, fileId, size);

            return file;
        }

        public SqlFileStream GetFileStream(SqlCommand command, CStorageEntityId fileId, FileAccess fileAccess)
        {
            CFileStreamInfo fileStreamInfo = GetFileStreamInfo(command, fileId);
            return new SqlFileStream(fileStreamInfo.Path, fileStreamInfo.TransactionContext, fileAccess);
        }

        public void UpdateFileSize(SqlCommand command, CStorageEntityId fileId, Int64 newSize)
        {
            var updateQuery = new UpdateFileSizeQuery(fileId, newSize);
            updateQuery.Execute(command);

            IStorageEntity file = GetStorageEntity(command, fileId);
            
            CStats stats = _statsProvider.GetStats(command, file.OwnerId);
            stats.AddUpdate(newSize); 
            _statsProvider.UpdateStats(command, stats); 
        }

        public async Task<CSharableEntity> AddFileAsync(Stream sourceStream, IStorageEntity parent, CUserId ownerId, String name, Int64 size)
        {
            CSharableEntity file = null;

            await _dbHelper.ExecuteTransactionAsync(async (command) =>
                {
                    file = await AddFileAsync(command, sourceStream, parent, ownerId, name, size);
                });

            return file;
        }
        
        public CSharableEntity AddDir(IStorageEntity parent, CUserId ownerId, String name)
        {
            CSharableEntity dir = null;
            _dbHelper.ExecuteTransaction((command) => dir = AddDir(command, parent, ownerId, name));
            return dir;
        }

        public List<CSharableEntity> GetOwnerChildren(CStorageEntityId parentId, CUserId userId)
        {
            var result = new List<CSharableEntity>();

            var query = new GetOwnerChildrenQuery(parentId, userId);
            List<CAccessWrapper> wrappers = _dbHelper.ExecuteQuery(query);

            foreach (var w in wrappers)
                result.Add(new CSharableEntity(w, _sharesProvider.GetShares(w.Entity.Id)));
            

            return result;
        }

        public List<CSharableEntity> GetSharedChildren(CStorageEntityId parentId, CUserId userId)
        {
            var result = new List<CSharableEntity>();

            var query = new GetSharedChildrenQuery(parentId, userId);
            List<CAccessWrapper> wrappers = _dbHelper.ExecuteQuery(query);

            foreach (var w in wrappers)
                result.Add(new CSharableEntity(w, _sharesProvider.GetShares(w.Entity.Id)));
            
            return result;
        }

        public List<CSearchResult> SearchOwnByName(String pattern, CStorageEntityId parentId, CUserId userId)
        {
            return SearchByName(pattern, parentId, userId, GetOwnerChildren);
        }

        public List<CSearchResult> SearchSharedByName(String pattern, CStorageEntityId parentId, CUserId userId)
        {
           return SearchByName(pattern, parentId, userId, GetSharedChildren);
        }

        public SortedDictionary<Int32, List<IStorageEntity>> GetChildrenAsc(CStorageEntityId entityId)
        {
            var childrenQuery = new GetChildrenQuery(entityId).Ascending();
            return _dbHelper.ExecuteQuery(childrenQuery);
        }

        public void DeleteEntity(CStorageEntityId entityId)
        {
            _dbHelper.ExecuteTransaction((command) => DeleteWithChildrenEntities(command, entityId));
        }

        public void RenameEntity(CStorageEntityId entityId, string name)
        {
            var query = new RenameEntityQuery(entityId, name);
            _dbHelper.ExecuteQuery(query);
        }
        
        public IStorageEntity GetStorageEntity(CStorageEntityId entityId)
        {
            var query = new GetStorageEntityQuery(entityId);
            return _dbHelper.ExecuteQuery(query);
        }

        public IStorageEntity GetStorageEntity(CStorageEntityId parentId, CUserId userId, string name)
        {
            var query = new GetStorageEntityQuery(parentId, userId, name);
            return _dbHelper.ExecuteQuery(query);
        }
        
        public Boolean EntityExists(CStorageEntityId parentId, CUserId userId, String name)
        {
            var query = new GetStorageEntityQuery(parentId, userId, name);
            return _dbHelper.ExecuteQuery(query) != null;
        }

        public async Task<CStorageEntityId> CopyEntity(CStorageEntityId entityId, CUserId userId, IStorageEntity newParent)
        {
            IStorageEntity entity = GetStorageEntity(entityId);

            CStorageEntityId newEntityId = new CStorageEntityId();
            switch (entity)
            {
                case CFile file:
                    await _dbHelper.ExecuteTransactionAsync(async (command) =>
                        newEntityId = await CopyFile(command, file, userId, newParent));
                    break;
                case CDirectory dir:
                    await _dbHelper.ExecuteTransactionAsync(async (command) => 
                        newEntityId = await CopyDir(command, dir, userId, newParent));
                    break;
                default:
                    throw new InvalidCastException(nameof(entity));
            }

            return newEntityId;
        }
    }
}
