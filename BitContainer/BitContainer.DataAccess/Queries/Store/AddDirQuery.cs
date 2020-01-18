using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    class AddDirQuery : AbstractWriteQuery
    {
        public Guid ParentId { get; set; }
        public Guid OwnerId { get; set; }
        public String Name { get; set; }
        
        private String QueryString =>
            $"INSERT INTO {DbNames.Entities} ({DbNames.Entities.ParentId}, {DbNames.Entities.OwnerId}, {DbNames.Entities.Name}) " +
            $"VALUES (" +  
            (ParentId.IsRootDir() ? "NULL" : $"@{nameof(ParentId)}") 
            + $", @{nameof(OwnerId)}, @{nameof(Name)});";

        public AddDirQuery(
            Guid parentId, 
            Guid ownerId, 
            String name)
        {
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;

            if (!ParentId.IsRootDir())
                command.Parameters.AddWithValue(nameof(ParentId), ParentId);

            command.Parameters.AddWithValue(nameof(OwnerId), OwnerId);
            command.Parameters.AddWithValue(nameof(Name), Name);
            return command;
        }
    }
}
