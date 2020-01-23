using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Store
{
    public class RenameEntityQuery : AbstractWriteQuery
    {
        public CStorageEntityId DirId { get; set; }
        public String NewName { get; set; }

        private static readonly String QueryString = 
            $"UPDATE {DbNames.Entities} SET {DbNames.Entities.PxName} = @{nameof(NewName)} " +
            $"WHERE {DbNames.Entities.PxId} = @{nameof(DirId)}";

        public RenameEntityQuery(CStorageEntityId id, String newName)
        {
            DirId = id;
            NewName = newName;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(NewName), NewName);
            command.Parameters.AddWithValue(nameof(DirId), DirId.ToGuid());
            return command;
        }
    }
}
