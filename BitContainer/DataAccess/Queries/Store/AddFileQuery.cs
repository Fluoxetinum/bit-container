using System;
using System.Data.Sql;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries
{
    class AddFileQuery : AbstractWriteQuery
    {
        public Guid ParentId { get; set; }
        public Guid OwnerId { get; set; }
        public String Name { get; set; }
        public Byte[] Data { get; set; }
        public Int32 Size { get; set; }

        private String QueryString =>
            $"INSERT INTO {DbNames.Entities} " +
            $"({DbNames.Entities.ParentId}, {DbNames.Entities.OwnerId}, " +
            $"{DbNames.Entities.Name}, {DbNames.Entities.Data}, {DbNames.Entities.Size}) " +
            $"VALUES (" + 
            (ParentId.IsRootDir() ? "NULL" : $"@{nameof(ParentId)}") 
            + $", @{nameof(OwnerId)}, @{nameof(Name)}, @{nameof(Data)}, @{nameof(Size)});";

        public AddFileQuery(
            Guid parentId,
            Guid ownerId,
            String name,
            Byte[] data)
        {
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Data = data;
            Size = data.Length;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            
            if (!ParentId.IsRootDir())
                command.Parameters.AddWithValue(nameof(ParentId), ParentId);
            
            command.Parameters.AddWithValue(nameof(OwnerId), OwnerId);
            command.Parameters.AddWithValue(nameof(Name), Name);
            command.Parameters.AddWithValue(nameof(Data), Data);
            command.Parameters.AddWithValue(nameof(Size), Size);
            return command;
        }
    }
}
