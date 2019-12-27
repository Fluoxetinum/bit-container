using System;
using System.Data.Sql;
using System.Data.SqlClient;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries
{
    class RemoveFileQuery : AbstractWriteQuery
    {
        public Guid Id { get; set; }

        private static readonly String QueryString = 
            $"DELETE FROM {DbNames.Entities} " +
            $"WHERE {DbNames.Entities.Id} = @{nameof(Id)};";

        public RemoveFileQuery(
            Guid id)
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
