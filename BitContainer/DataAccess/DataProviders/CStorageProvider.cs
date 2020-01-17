using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Get;
using BitContainer.DataAccess.Queries.Share;
using BitContainer.DataAccess.Queries.Store;

namespace BitContainer.DataAccess.DataProviders
{
    public class CStorageProvider : IStorageProvider
    {
        public IStorageEntitiesProvider StorageEntities { get; set; }
        public ISharesProvider Shares { get; set; }

        public CStorageProvider()
        {
            StorageEntities = new CStorageEntitiesProvider();
            Shares = new CSharesProvider();
        }

        public List<CSearchResult> SearchOwnEntitiesByName(String pattern, Guid parentId, Guid ownerId)
        {
            List<CSearchResult> result = new List<CSearchResult>();

            CDbHelper.ExecuteTransaction(executionAlgorithm: (command) =>
            {
                result = SearchOwnEntitiesTransaction(pattern, parentId, ownerId, command);
            });

            return result;
        }

        public List<CSearchResult> SearchOwnEntitiesTransaction(String pattern, Guid parentId, Guid ownerId,
            SqlCommand command)
        {
            List<CSearchResult> searchResult = new List<CSearchResult>(); 

            Dictionary<Guid,COwnStorageEntity> flicked = new Dictionary<Guid, COwnStorageEntity>();
            Stack<COwnStorageEntity> entities = new Stack<COwnStorageEntity>();

            foreach (var children in GetChildren(command, parentId, ownerId))
                entities.Push(children);

            List<COwnStorageEntity> result = new List<COwnStorageEntity>();

            while (entities.Count > 0)
            {
                COwnStorageEntity entity = entities.Pop();

                if (Regex.IsMatch(entity.Entity.Name, $".*{pattern}.*", RegexOptions.IgnoreCase))
                    result.Add(entity);
                
                Guid newParentId = entity.Entity.Id;

                foreach (var children in GetChildren(command, newParentId, ownerId))
                    entities.Push(children);

                flicked[newParentId] = entity;
            }
            
            foreach (var r in result)
            {
                LinkedList<Guid> downpath = new LinkedList<Guid>();
                
                Guid currentPurentId = r.Entity.ParentId;
                while (currentPurentId != parentId)
                {
                    COwnStorageEntity currentParent = flicked[currentPurentId];
                    downpath.AddFirst(currentPurentId);
                    currentPurentId = currentParent.Entity.ParentId;
                }
                downpath.AddFirst(parentId);
                downpath.AddLast(r.Entity.Id);

                searchResult.Add(new CSearchResult()
                {
                    AccessWrapper = r,
                    DownPath = downpath
                });
            }

            return searchResult;


        }

        private List<COwnStorageEntity> GetChildren(SqlCommand command, Guid parentId, Guid ownerId)
        {
            var query = new GetOwnerChildrenQuery(parentId, ownerId);
            return query.Execute(command);
        }


        public List<CSearchResult> SearchRestrictedEntitiesByName(String pattern, Guid parentId, Guid ownerId)
        {
            List<CSearchResult> result = new List<CSearchResult>();

            CDbHelper.ExecuteTransaction(executionAlgorithm: (command) =>
                {
                    result = SearchRestrictedEntitiesByNameTransaction(pattern, parentId, ownerId, command);
                });

            return result;
        }

        public  List<CSearchResult> SearchRestrictedEntitiesByNameTransaction
            (String pattern, Guid parentId, Guid ownerId, SqlCommand command)
        {
            List<CSearchResult> searchResult = new List<CSearchResult>(); 

            Dictionary<Guid,CRestrictedStorageEntity> flicked = new Dictionary<Guid, CRestrictedStorageEntity>();
            Stack<CRestrictedStorageEntity> entities = new Stack<CRestrictedStorageEntity>();

            HashSet<Guid> upMostParents = new HashSet<Guid>();

            upMostParents.Add(parentId);

            if (parentId.IsRootDir())
            {
                var query = new GetSharedRootChildrenQuery(ownerId);
                foreach (var children in query.Execute(command))
                {
                    upMostParents.Add(children.Entity.ParentId);
                    entities.Push(children);
                }
            }
            else
            {
                foreach (var children in GetSharedChildren(command, parentId, ownerId))
                    entities.Push(children);
            }

            List<CRestrictedStorageEntity> result = new List<CRestrictedStorageEntity>();

            while (entities.Count > 0)
            {
                CRestrictedStorageEntity entity = entities.Pop();

                if (Regex.IsMatch(entity.Entity.Name, $".*{pattern}.*", RegexOptions.IgnoreCase))
                    result.Add(entity);
                
                Guid newParentId = entity.Entity.Id;

                foreach (var children in GetSharedChildren(command, newParentId, ownerId))
                    entities.Push(children);

                flicked[newParentId] = entity;
            }

            LinkedList<Guid> downpath = new LinkedList<Guid>();
            
            foreach (var r in result)
            {
                
                Guid currentPurentId = r.Entity.ParentId;
                while (!upMostParents.Contains(currentPurentId))
                {
                    CRestrictedStorageEntity currentParent = flicked[currentPurentId];
                    downpath.AddFirst(currentPurentId);
                    currentPurentId = currentParent.Entity.ParentId;
                }
                downpath.AddFirst(currentPurentId);
                downpath.AddLast(r.Entity.Id);

                searchResult.Add(new CSearchResult()
                {
                    AccessWrapper = r,
                    DownPath = downpath
                });
            }

            return searchResult;
        }

        private List<CRestrictedStorageEntity> GetSharedChildren(SqlCommand command, Guid parentId, Guid ownerId)
        {
            List<CRestrictedStorageEntity> result = new List<CRestrictedStorageEntity>();

            var shareQuery = new GetShareByIdQuery(parentId, ownerId);
            CShare share = shareQuery.Execute(command);

            if (share == null) return result;

            var query = new GetSharedChildrenEntitiesQuery(ownerId, 
                parentId,
                share.RestrictedAccessType);

            result = query.Execute(command);

            return result;
        }


    }
}
