using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Auth
{
    public class RemoveUserQuery : AbstractWriteQuery
    {
        public CUserId UserId { get; set; }

        private static readonly String QueryString = 
            $"DELETE FROM {DbNames.Users} WHERE {DbNames.Users.PxId} = @{nameof(UserId)};";

        public RemoveUserQuery(CUserId userId)
        {
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(UserId), UserId.ToGuid());
            return command;
        }
    }
}
