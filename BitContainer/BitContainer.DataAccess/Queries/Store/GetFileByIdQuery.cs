using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    public class GetFileByIdQuery : AbstractScalarQuery<CFile>
    {
        public Guid Id { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Entities.Id}, {DbNames.Entities.ParentId}, {DbNames.Entities.OwnerId}, " +
            $"{DbNames.Entities.Name}, {DbNames.Entities.Created}, {DbNames.Entities.Size} " +
            $"FROM {DbNames.Entities} " +
            $"WHERE {DbNames.Entities.Id} = @{nameof(Id)}; ";

        public GetFileByIdQuery(Guid id) 
            : base(new CFileMapper())
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
