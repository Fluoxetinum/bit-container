using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Mappers.StorageEntities;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    public class GetFileWithOwner: AbstractScalarQuery<CFile>
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Entities.Id}, {DbNames.Entities.ParentId}, {DbNames.Entities.OwnerId}, " +
            $"{DbNames.Entities.Name}, {DbNames.Entities.Created}, {DbNames.Entities.Size} " +
            $"FROM {DbNames.Entities} " +
            $"WHERE {DbNames.Entities.Id} = @{nameof(Id)} " +
            $"AND {DbNames.Entities.OwnerId} = @{nameof(OwnerId)}; ";

        public GetFileWithOwner(Guid id, Guid ownerId) 
            : base(new CFileMapper())
        {
            Id = id;
            OwnerId = ownerId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(Id), Id);
            command.Parameters.AddWithValue(nameof(OwnerId), OwnerId);
            return command;
        }
    
    }
}
