using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Mappers.StorageEntities;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    public class GetDirByIdQuery : AbstractScalarQuery<CDirectory>
    {
        public Guid Id { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Entities.Id}, {DbNames.Entities.ParentId}, {DbNames.Entities.OwnerId}, " +
            $"{DbNames.Entities.Name}, {DbNames.Entities.Created} " +
            $"FROM {DbNames.Entities} " +
            $"WHERE {DbNames.Entities.Id} = @{nameof(Id)};";

        public GetDirByIdQuery(Guid id) 
            : base(new CDirectoryMapper())
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
