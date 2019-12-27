using System;
using System.Data.Sql;
using System.Data.SqlClient;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries
{
    class AddNewUserQuery : AbstractWriteQuery
    {
        public String Name { get; set; }
        public Byte[] PasswordHash { get; set; }
        public Byte[] Salt { get; set; } 

        private static readonly String QueryString =
            $"INSERT INTO {DbNames.Users} " +
            $"({DbNames.Users.Name}, {DbNames.Users.PassHash}, {DbNames.Users.Salt}) " +
            $"VALUES (@{nameof(Name)}, @{nameof(PasswordHash)}, @{nameof(Salt)});";

        public AddNewUserQuery(String name, Byte[] passwordHash, Byte[] salt)
        {
            Name = name;
            PasswordHash = passwordHash;
            Salt = salt;
        }
        
        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(Name), Name);
            command.Parameters.AddWithValue(nameof(PasswordHash), 
                Convert.ToBase64String(PasswordHash));
            command.Parameters.AddWithValue(nameof(Salt), 
                Convert.ToBase64String(Salt));
            return command;
        }
    }
}
