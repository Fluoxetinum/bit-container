﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BitContainer.DataAccess.Queries.Base
{
    public class DbNames
    {
        public static AccessTypesTableNames AccessTypes { get; } = new AccessTypesTableNames();
        public static EntitiesTableNames Entities { get; } = new EntitiesTableNames();
        public static SharesTableNames Shares { get; } = new SharesTableNames();
        public static StatsTableNames Stats { get; } = new StatsTableNames();
        public static UsersTableNames Users { get; } = new UsersTableNames();

        public static GetShareByIdSpNames GetShareById { get; } = new GetShareByIdSpNames();
        public static GetOwnerDirChildrenSpNames GetOwnerDirChildren { get; } = new GetOwnerDirChildrenSpNames();
        public static GetSharedChildrenSpNames GetSharedChildren { get; } = new GetSharedChildrenSpNames();
        public static GetSharedRootChildrenSpNames GetSharedRootChildren { get; } = new GetSharedRootChildrenSpNames();
        public static RemoveDirSpNames RemoveDir { get; } = new RemoveDirSpNames();
        public static GetAllDirChildrenSpNames GetAllDirChildren { get; } = new GetAllDirChildrenSpNames();
        public static SearchOwnByNameSpNames SearchOwnByName { get; } = new SearchOwnByNameSpNames();
        public static SearchSharedByNameSpNames SearchSharedByName { get; } = new SearchSharedByNameSpNames();
        public static SearchSharedRootByNameSpNames SearhcSharedRootByName { get; } = new SearchSharedRootByNameSpNames();

        #region Tables

        public class AccessTypesTableNames : DbEntityName
        {
            public AccessTypesTableNames():base(entityName:"AccessTypes"){}

            public String Id => $"{EntityName}.ID";
            public String Name => $"{EntityName}.Name";
        }

        public class EntitiesTableNames : DbEntityName
        {
            public EntitiesTableNames():base(entityName:"StorageEntities"){}
            public String Id => $"{EntityName}.ID";
            public String ParentId => $"{EntityName}.ParentID";
            public String OwnerId => $"{EntityName}.OwnerID";
            public String Name => $"{EntityName}.Name";
            public String Created => $"{EntityName}.Created";
            public String Data => $"{EntityName}.Data";
            public String Size => $"{EntityName}.Size";
        }

        public class SharesTableNames : DbEntityName
        {
            public SharesTableNames():base(entityName:"Shares"){}
            public String UserApprovedId => $"{EntityName}.UserApprovedID";
            public String AccessTypeId => $"{EntityName}.AccessTypeID";
            public String EntityId => $"{EntityName}.EntityID";
        }

        public class StatsTableNames : DbEntityName
        {
            public StatsTableNames():base(entityName:"Stats"){}
            public String Id => $"{EntityName}.ID";
            public String UserId => $"{EntityName}.UserID";
            public String FilesCount => $"{EntityName}.FilesCount";
            public String DirsCount => $"{EntityName}.DirectoriesCount";
            public String StorageSize => $"{EntityName}.StorageSize";
        }

        public class UsersTableNames : DbEntityName
        {
            public UsersTableNames():base(entityName:"Users"){}

            public String Id => $"{EntityName}.ID";
            public String Name => $"{EntityName}.Name";
            public String PassHash => $"{EntityName}.PasswordHash";
            public String Salt => $"{EntityName}.Salt";
        }

        #endregion

        #region StoredProcedures

        public class GetShareByIdSpNames : DbEntityName
        {
            public GetShareByIdSpNames() : base(entityName:"[sp_GetShareById]"){}

            public String PersonId => "PersonID";
            public String EntityId => "EntityID";
        }
        
        public class GetOwnerDirChildrenSpNames : DbEntityName
        {
            public GetOwnerDirChildrenSpNames() : base(entityName:"[sp_GetOwnerDirChildren]"){}

            public String ParentId => "ParentID";
            public String OwnerId => "OwnerID";
        }
        
        public class GetSharedChildrenSpNames : DbEntityName
        {
            public GetSharedChildrenSpNames() : base(entityName:"[sp_GetSharedDirChildren]"){}

            public String ParentId => "ParentID";
            public String PersonId => "PersonID";
            public String ParentAccess => "ParentAccess";
        }

        public class GetSharedRootChildrenSpNames : DbEntityName
        {
            public GetSharedRootChildrenSpNames() : base(entityName:"[sp_GetSharedRootChildren]"){}

            public String ParentId => "ParentID";
            public String PersonId => "PersonID";
            public String ParentAccess => "ParentAccess";
        }

        public class RemoveDirSpNames : DbEntityName
        {
            public RemoveDirSpNames() : base(entityName:"[sp_RemoveDir]"){}

            public String DirId => "DirID";
        }

        public class GetAllDirChildrenSpNames : DbEntityName
        {
            public GetAllDirChildrenSpNames() : base(entityName:"[sp_GetAllDirChildren]"){}
            public String DirId => "DirID";
        }

        public class SearchOwnByNameSpNames : DbEntityName
        {
            public SearchOwnByNameSpNames() : base(entityName:"[sp_SearchOwnByName]"){}
            public String Pattern => "Pattern";
            public String UserId => "UserID";
            public String ParentId => "ParentID";
        }

        public class SearchSharedByNameSpNames : DbEntityName
        {
            public SearchSharedByNameSpNames() : base(entityName: "[sp_SearchSharedByName]") {}
            public String Pattern => "Pattern";
            public String UserId => "UserID";
            public String ParentId => "ParentID";
            public String ParentAccess => "ParentAccess";
        }

        public class SearchSharedRootByNameSpNames : DbEntityName
        {
            public SearchSharedRootByNameSpNames() : base(entityName: "[sp_SearchSharedRootByName]") {}
            public String UserId => "UserID";
            public String Pattern => "Pattern";
        }

        #endregion

        public class DbEntityName
        {
            public String EntityName { get; }

            public DbEntityName(String entityName)
            {
                EntityName = entityName;
            }

            public override string ToString()
            {
                return EntityName;
            }
        }
    }
}
