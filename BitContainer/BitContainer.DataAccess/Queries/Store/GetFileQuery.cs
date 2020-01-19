using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Mappers.StorageEntities;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    class GetFileQuery : AbstractScalarQuery<CFile>
    {
        public Guid ParentId { get; set; }
        public Guid OwnerId { get; set; }
        public String Name { get; set; }

        private String QueryString => 
            $"SELECT {DbNames.Entities.Id}, {DbNames.Entities.ParentId}, {DbNames.Entities.OwnerId}, " +
            $"{DbNames.Entities.Name}, {DbNames.Entities.Created}, {DbNames.Entities.Size} " +
            $"FROM {DbNames.Entities} " +
            $"WHERE {DbNames.Entities.Name} = @{nameof(Name)} " +
            $"AND {DbNames.Entities.OwnerId} = @{nameof(OwnerId)} " +
            $"AND {DbNames.Entities.ParentId} " + 
            (ParentId.IsRootDir() ? "IS NULL" : $"= @{nameof(ParentId)} ");

        public GetFileQuery(
            Guid parentId,
            Guid ownerId,
            String name) : base(new CFileMapper())
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
