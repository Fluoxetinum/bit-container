using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    public class RenameEntityQuery : AbstractWriteQuery
    {
        public Guid DirId { get; set; }
        public String NewName { get; set; }

        private static readonly String QueryString = 
            $"UPDATE {DbNames.Entities} SET {DbNames.Entities.Name} = @{nameof(NewName)} " +
            $"WHERE {DbNames.Entities.Id} = @{nameof(DirId)}";

        public RenameEntityQuery(Guid id, String newName)
        {
            DirId = id;
            NewName = newName;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(NewName), NewName);
            command.Parameters.AddWithValue(nameof(DirId), DirId);
            return command;
        }
    }
}
