using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Auth
{
    public class RemoveUserQuery : AbstractWriteQuery
    {
        public Guid UserId { get; set; }

        private static readonly String QueryString = 
            $"DELETE FROM {DbNames.Users} WHERE {DbNames.Users.Id} = @{nameof(UserId)};";

        public RemoveUserQuery(Guid userId)
        {
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(UserId), UserId);
            return command;
        }
    }
}
