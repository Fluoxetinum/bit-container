using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.StorageEntites
{
    public class UpdateFileSizeQuery : AbstractWriteQuery
    {
        public CStorageEntityId FileId { get; set; }
        public Int64 NewSize { get; set; }

        private static readonly String QueryString =
            $"UPDATE {DbNames.Entities} SET Size = @{nameof(NewSize)} " +
            $"WHERE ID = @{nameof(FileId)}";

        public UpdateFileSizeQuery(CStorageEntityId fileId, Int64 newSize)
        {
            FileId = fileId;
            NewSize = newSize;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(NewSize), NewSize);
            command.Parameters.AddWithValue(nameof(FileId), FileId.ToGuid());
            return command;
        }
    }
}
