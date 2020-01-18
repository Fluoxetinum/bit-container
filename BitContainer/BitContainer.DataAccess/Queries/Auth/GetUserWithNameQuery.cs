using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Auth
{
    class GetUserWithNameQuery: AbstractScalarQuery<CUser>
    {
        public String Name { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Users.Id}, {DbNames.Users.Name} " +
            $"FROM {DbNames.Users} " +
            $"WHERE {DbNames.Users.Name} = @{nameof(Name)};";

        public GetUserWithNameQuery(String name) 
            : base(new CUserMapper())
        {
            Name = name;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(Name), Name);
            return command;
        }
    }
}
