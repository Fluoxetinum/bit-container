using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    public class UpdateFileSizeQuery : AbstractWriteQuery
    {
        public Guid FileId { get; set; }
        public Int64 NewSize { get; set; }

        private static readonly String QueryString =
            $"UPDATE StorageEntities SET Size = @{nameof(NewSize)} " +
            $"WHERE ID = @{nameof(FileId)}";

        public UpdateFileSizeQuery(Guid fileId, Int64 newSize)
        {
            FileId = fileId;
            NewSize = newSize;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(NewSize), NewSize);
            command.Parameters.AddWithValue(nameof(FileId), FileId);
            return command;
        }
    }
}
