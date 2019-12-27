using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Share
{
    public class RemoveShareQuery : AbstractWriteQuery
    {
        public Guid DirId { get; set; }
        public Guid PersonId { get; set; }

        private static readonly String QueryString = 
            $"DELETE FROM {DbNames.Shares} " +
            $"WHERE {DbNames.Shares.EntityId} = @{nameof(DirId)} " +
            $"AND {DbNames.Shares.PersonApprovedId} = @{nameof(PersonId)};";

        public RemoveShareQuery(Guid dirId, Guid personId)
        {
            DirId = dirId;
            PersonId = personId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(DirId), DirId);
            command.Parameters.AddWithValue(nameof(PersonId), PersonId);
            return command;
        }
    }
}
