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
    public class GetStorageEntityQuery : AbstractScalarQuery<IStorageEntity>
    {
        public Guid Id { get; set; }

        private static readonly String QueryString =
            $"SELECT {DbNames.Entities.Id}, {DbNames.Entities.ParentId}, {DbNames.Entities.OwnerId}, " +
            $"{DbNames.Entities.Name}, {DbNames.Entities.Created}, {DbNames.Entities.Size} " +
            $"FROM {DbNames.Entities} WHERE ID = @{nameof(Id)}";

        public GetStorageEntityQuery(Guid id) : base(new CStorageEntityMapper())
        {
            Id = id;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(Id), Id);
            return command;
        }
    }
}
