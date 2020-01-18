using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Auth
{
    class GetSaltQuery : AbstractScalarQuery<Byte[]>
    {
        public String UserName { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Users.Salt} " +
            $"FROM {DbNames.Users} " +
            $"WHERE {DbNames.Users.Name} = @{nameof(UserName)}";

        public GetSaltQuery(String userName) : base(new CBytesMapper()) 
        {
            UserName = userName;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(UserName), UserName);
            return command;
        }
    }
}
