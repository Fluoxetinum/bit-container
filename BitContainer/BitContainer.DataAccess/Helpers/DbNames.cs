using System;

namespace BitContainer.DataAccess.Helpers
{
    public class DbNames
    {
        public static string LogsDbName => "BITCONTAINER_LOGS_DB";
        public static string StorageDbName => "BITCONTAINER_STORAGE_DB";
        public static string AuthDbName => "BITCONTAINER_AUTH_DB";

        public static AccessTypesTableNames AccessTypes { get; } = new AccessTypesTableNames();
        public static EntitiesTableNames Entities { get; } = new EntitiesTableNames();
        public static SharesTableNames Shares { get; } = new SharesTableNames();
        public static StatsTableNames Stats { get; } = new StatsTableNames();
        public static UsersTableNames Users { get; } = new UsersTableNames();
        public static LogsTableName Logs { get; } = new LogsTableName();

        #region Tables

        public class AccessTypesTableNames : DbEntityName
        {
            public AccessTypesTableNames() : base(entityName: "AccessTypes") { }

            public String Id => $"ID";
            public String Name => $"Name";

            public String PxId => $"{EntityName}.{Id}";
            public String PxName => $"{EntityName}.{Name}";
        }

        public class EntitiesTableNames : DbEntityName
        {
            public EntitiesTableNames() : base(entityName: "StorageEntities") { }

            public String Id => $"ID";
            public String ParentId => $"ParentID";
            public String OwnerId => $"OwnerID";
            public String Name => $"Name";
            public String Created => $"Created";
            public String Data => $"Data";
            public String Size => $"Size";

            public String PxId => $"{EntityName}.{Id}";
            public String PxParentId => $"{EntityName}.{ParentId}";
            public String PxOwnerId => $"{EntityName}.{OwnerId}";
            public String PxName => $"{EntityName}.{Name}";
            public String PxCreated => $"{EntityName}.{Created}";
            public String PxData => $"{EntityName}.{Data}";
            public String PxSize => $"{EntityName}.{Size}";
        }

        public class SharesTableNames : DbEntityName
        {
            public SharesTableNames() : base(entityName: "Shares") { }

            public String UserApprovedId => $"UserApprovedID";
            public String AccessTypeId => $"AccessTypeID";
            public String EntityId => $"EntityID";

            public String PxUserApprovedId => $"{EntityName}.{UserApprovedId}";
            public String PxAccessTypeId => $"{EntityName}.{AccessTypeId}";
            public String PxEntityId => $"{EntityName}.{EntityId}";
        }

        public class StatsTableNames : DbEntityName
        {
            public StatsTableNames() : base(entityName: "Stats") { }

            public String UserId => $"UserID";
            public String FilesCount => $"FilesCount";
            public String DirsCount => $"DirectoriesCount";
            public String StorageSize => $"StorageSize";

            public String PxUserId => $"{EntityName}.{UserId}";
            public String PxFilesCount => $"{EntityName}.{FilesCount}";
            public String PxDirsCount => $"{EntityName}.{DirsCount}";
            public String PxStorageSize => $"{EntityName}.{StorageSize}";
        }

        public class UsersTableNames : DbEntityName
        {
            public UsersTableNames() : base(entityName: "Users") { }

            public String Id => $"ID";
            public String Name => $"Name";
            public String PassHash => $"PasswordHash";
            public String Salt => $"Salt";

            public String PxId => $"{EntityName}.{Id}";
            public String PxName => $"{EntityName}.{Name}";
            public String PxPassHash => $"{EntityName}.{PassHash}";
            public String PxSalt => $"{EntityName}.{Salt}";
        }

        public class LogsTableName : DbEntityName
        {
            public LogsTableName() : base(entityName: "Logs") { }

            public String LogLevel => $"LogLevel";
            public String Message => $"Message";
            public String Exception => $"Exception";

            public String PxLogLevel => $"{EntityName}.{LogLevel}";
            public String PxMessage => $"{EntityName}.{Message}";
            public String PxException => $"{EntityName}.{Exception}";
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
