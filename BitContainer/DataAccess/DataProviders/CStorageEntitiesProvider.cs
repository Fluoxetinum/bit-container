using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.DataAccess.Queries.Get;
using BitContainer.DataAccess.Queries.Share;
using BitContainer.DataAccess.Queries.Stats;
using BitContainer.DataAccess.Queries.Store;

namespace BitContainer.DataAccess.DataProviders
{
    public class CStorageEntitiesProvider : IStorageEntitiesProvider
    {
        public List<COwnStorageEntity> GetOwnerChildren(Guid parentDirectoryId, Guid ownerId)
        {
            var query = new GetOwnerChildrenQuery(parentDirectoryId, ownerId);
            return CDbHelper.ExecuteQuery(query);
        }

        public List<CRestrictedStorageEntity> GetSharedChildren(Guid parentDirectoryId, Guid userId)
        {
            List<CRestrictedStorageEntity> result = new List<CRestrictedStorageEntity>();

            CDbHelper.ExecuteTransaction(executionAlgorithm: (command) =>
            {
                if (parentDirectoryId.IsRootDir())
                {
                    var query = new GetSharedRootChildrenQuery(userId);
                    result = query.Execute(command);
                }
                else
                {
                    var shareQuery = new GetShareByIdQuery(parentDirectoryId, userId);
                    CShare share = shareQuery.Execute(command);

                    if (share == null) return;
                    
                    var query = new GetSharedChildrenEntitiesQuery(userId, 
                        parentDirectoryId, 
                        share.RestrictedAccessType);
                    result = query.Execute(command);
                }
                
            });

            return result;
        }

        public CDirectory GetDir(Guid parentId, Guid userId, string name)
        {
            var query = new GetDirQuery(parentId, userId, name);
            return CDbHelper.ExecuteQuery(query);
        }

        public CDirectory GetDir(Guid id)
        {
            var query = new GetDirByIdQuery(id);
            return CDbHelper.ExecuteQuery(query);
        }

        public CDirectory AddDir(Guid parentId, Guid userId, String name)
        {
            CDirectory result = null;

            CDbHelper.ExecuteTransaction(executionAlgorithm: (command) =>
             {
                 Guid ownerId = userId;

                 if (!parentId.IsRootDir())
                 {
                     var dirQuery = new GetDirByIdQuery(parentId);
                     CDirectory parent = dirQuery.Execute(command);
                     ownerId = parent.OwnerId;
                 }

                 var query = new AddDirQuery(parentId, ownerId, name);
                 query.Execute(command);

                 var statsQuery = new GetUserStatsQuery(ownerId);
                 CUserStats stats = statsQuery.Execute(command);
                 stats.DirsCount++;

                 var updateQuery = new UpdateUserStatsQuery(stats);
                 updateQuery.Execute(command);

                 var newDirQuery = new GetDirQuery(parentId, ownerId, name);
                 result = newDirQuery.Execute(command);
             });

            return result;
        }

        public Int32 RenameEntity(Guid id, string name)
        {
            var query = new RenameEntityQuery(id, name);
            return CDbHelper.ExecuteQuery(query);
        }

        public Int32 DeleteDir(Guid id)
        {
            Int32 result = -1;

            CDbHelper.ExecuteTransaction(executionAlgorithm: (command) =>
             {
                 var dirQuery = new GetDirByIdQuery(id);
                 CDirectory dir = dirQuery.Execute(command);

                 var query = new RemoveDirQuery(id);
                 result = query.Execute(command);

                 var statsQuery = new GetUserStatsQuery(dir.OwnerId);
                 CUserStats stats = statsQuery.Execute(command);
                 stats.DirsCount--;

                 var updateQuery = new UpdateUserStatsQuery(stats);
                 updateQuery.Execute(command);
             });

            return result;
        }

        public Dictionary<Int32, List<IStorageEntity>> GetAllChildren(Guid dirId)
        {
            Dictionary<Int32, List<IStorageEntity>> result = new Dictionary<int, List<IStorageEntity>>();

            CDbHelper.ExecuteTransaction(executionAlgorithm: (command) =>
            {
                var dirQuery = new GetDirByIdQuery(dirId);
                CDirectory dir = dirQuery.Execute(command);

                var childrenQuery = new GetDirChildrenQuery(dir.Id);
                List<CChild> children = childrenQuery.Execute(command);

                foreach (var child in children)
                {
                    if (!result.ContainsKey(child.Level))
                        result[child.Level] = new List<IStorageEntity>();

                    switch (child.Type)
                    {
                        case EEntityType.File:

                            var fileQuery = new GetFileByIdQuery(child.Id);
                            CFile f = fileQuery.Execute(command);
                            result[child.Level].Add(f);

                            break;
                        case EEntityType.Directory:

                            var getDirQuery = new GetDirByIdQuery(child.Id);
                            CDirectory d = getDirQuery.Execute(command);
                            result[child.Level].Add(d);

                            break;
                        default:
                            throw new NotSupportedException("Unexpected entity type.");
                    }
                }
            });

            return result;
        }

        public Int32 CopyDir(Guid id, Guid copierId, Guid newParentId)
        {
            Int32 result = -1;

            CDbHelper.ExecuteTransaction(executionAlgorithm: (command) =>
             {
                 CopyDirTransaction(command, id, copierId, newParentId);
             });

            return result;
        }

        public void CopyDirTransaction(SqlCommand command, Guid id, Guid copierId, Guid newParentId)
        {
            var dirQuery = new GetDirByIdQuery(id);
            CDirectory dir = dirQuery.Execute(command);

            var childrenQuery = new GetDirChildrenQuery(dir.Id);
            List<CChild> children = childrenQuery.Execute(command);

            var grouppedByLevel = children.GroupBy(ch => ch.Level).OrderBy(g => g.Key);

            Dictionary<Guid, Guid> oldStartParents = new Dictionary<Guid, Guid>();
            oldStartParents[dir.ParentId] = newParentId;
            foreach (var groupping in grouppedByLevel)
            {
                foreach (var child in groupping)
                {
                    switch (child.Type)
                    {
                        case EEntityType.Directory:

                            var getDirQuery = new GetDirByIdQuery(child.Id);
                            CDirectory d = getDirQuery.Execute(command);

                            Guid newDirParent = oldStartParents[d.ParentId];

                            var addDirQuery = new AddDirQuery(newDirParent, copierId, d.Name);
                            addDirQuery.Execute(command);

                            var getNewDirQuery = new GetDirQuery(newDirParent, copierId, d.Name);
                            CDirectory newD = getNewDirQuery.Execute(command);

                            var statsQuery = new GetUserStatsQuery(newD.OwnerId);
                            CUserStats stats = statsQuery.Execute(command);
                            stats.DirsCount++;

                            var updateQuery = new UpdateUserStatsQuery(stats);
                            updateQuery.Execute(command);

                            oldStartParents[d.Id] = newD.Id;

                            break;
                        case EEntityType.File:

                            var fileQuery = new GetFileByIdQuery(child.Id);
                            CFile f = fileQuery.Execute(command);

                            var dataQuery = new GetAllFileDataQuery(f.Id);
                            Byte[] data = dataQuery.Execute(command);

                            Guid newFileParent = oldStartParents[f.ParentId];

                            var addFileQuery = new AddFileQuery(newFileParent, copierId, f.Name, data);
                            addFileQuery.Execute(command);

                            var getFileQuery = new GetFileQuery(newFileParent, copierId, f.Name);
                            CFile newFile = getFileQuery.Execute(command);

                            var userStatsQuery = new GetUserStatsQuery(newFile.OwnerId);
                            CUserStats userStats = userStatsQuery.Execute(command);

                            userStats.FilesCount++;
                            userStats.StorageSize += newFile.Size;

                            var updateStatsQuery = new UpdateUserStatsQuery(userStats);
                            updateStatsQuery.Execute(command);

                            oldStartParents[f.Id] = newFile.Id;

                            break;
                        default:
                            throw new NotSupportedException("Unexpected entity type.");
                    }
                }
            }
        }

        public Byte[] GetAllFileData(Guid fileId)
        {
            var query = new GetAllFileDataQuery(fileId);
            return CDbHelper.ExecuteQuery(query);
        }

        public CFile GetFile(Guid parentId, Guid getterId, String name)
        {
            var query = new GetFileQuery(parentId, getterId, name);
            return CDbHelper.ExecuteQuery(query);
        }

        public CFile GetFile(Guid id)
        {
            var query = new GetFileByIdQuery(id);
            return CDbHelper.ExecuteQuery(query);
        }

        public CFile AddFile(Guid parentId, Guid adderId, String name, Byte[] data)
        {
            CFile result = null;

            CDbHelper.ExecuteTransaction(executionAlgorithm:(command) =>
            {
                Guid ownerId = adderId;

                if (!parentId.IsRootDir())
                {
                    var dirQuery = new GetDirByIdQuery(parentId);
                    CDirectory parent = dirQuery.Execute(command);
                    ownerId = parent.OwnerId;
                }

                var query = new AddFileQuery(parentId, ownerId, name, data);
                query.Execute(command);

                var fileQuery = new GetFileQuery(parentId, ownerId, name);
                CFile file = fileQuery.Execute(command);

                var userStatsQuery = new GetUserStatsQuery(ownerId);
                CUserStats userStats = userStatsQuery.Execute(command);

                userStats.FilesCount++;
                userStats.StorageSize += file.Size;

                var updateStatsQuery = new UpdateUserStatsQuery(userStats);
                updateStatsQuery.Execute(command);

                result = file;
            });

            return result;
        }

        public Int32 DeleteFile(Guid id)
        {
            Int32 result = -1;

            CDbHelper.ExecuteTransaction(executionAlgorithm:(command) =>
            {
                var fileQuery = new GetFileByIdQuery(id);
                CFile file = fileQuery.Execute(command);

                var query = new RemoveFileQuery(id);
                query.Execute(command);
                
                var userStatsQuery = new GetUserStatsQuery(file.OwnerId);
                CUserStats userStats = userStatsQuery.Execute(command);

                userStats.FilesCount--;
                userStats.StorageSize -= file.Size;

                var updateStatsQuery = new UpdateUserStatsQuery(userStats);
                result = updateStatsQuery.Execute(command);
            });

            return result;
        }

        public Int32 CopyFile(Guid id, Guid copierId, Guid newParentId)
        {
            Int32 result = -1;

            CDbHelper.ExecuteTransaction(executionAlgorithm:(command) =>
            {
                var fileQuery = new GetFileByIdQuery(id);
                CFile file = fileQuery.Execute(command);

                var dataQuery = new GetAllFileDataQuery(file.Id);
                Byte[] data = dataQuery.Execute(command);
                
                var addFileQuery = new AddFileQuery(newParentId, copierId, file.Name, data);
                addFileQuery.Execute(command);

                var getFileQuery = new GetFileQuery(newParentId, copierId, file.Name);
                CFile newFile = getFileQuery.Execute(command);

                var userStatsQuery = new GetUserStatsQuery(newFile.OwnerId);
                CUserStats userStats = userStatsQuery.Execute(command);

                userStats.FilesCount++;
                userStats.StorageSize += newFile.Size;

                var updateStatsQuery = new UpdateUserStatsQuery(userStats);
                result = updateStatsQuery.Execute(command);
            });

            return result;
        }

    }
}
