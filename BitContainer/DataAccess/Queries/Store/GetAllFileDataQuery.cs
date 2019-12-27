using System;
using System.Data.Sql;
using System.Data.SqlClient;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries
{
    public class GetAllFileDataQuery : AbstractScalarQuery<Byte[]>
    {
        public Guid FileId { get; set; }

        private static readonly String _queryString =
            $"SELECT {DbNames.Entities.Data}, {DbNames.Entities.Size} " +
            $"FROM {DbNames.Entities} WHERE {DbNames.Entities.Id} = @{nameof(FileId)};";

        public GetAllFileDataQuery(
            Guid fileId)
            : base(new CFileDataMapper())
        {
            FileId = fileId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = _queryString;
            command.Parameters.AddWithValue(nameof(FileId), FileId);
            return command;
        }
    }
}
