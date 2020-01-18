﻿using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Auth
{
    class GetUserWithCredentialsQuery : AbstractScalarQuery<CUser>
    {
        public String Name { get; set; }
        public Byte[] PasswordHash { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Users.Id}, {DbNames.Users.Name} " +
            $"FROM {DbNames.Users} " +
            $"WHERE {DbNames.Users.Name} = @{nameof(Name)} " +
            $"AND {DbNames.Users.PassHash} = @{nameof(PasswordHash)}";

        public GetUserWithCredentialsQuery(
            String name,
            Byte[] passwordHash) 
            : base(new CUserMapper())
        {
            Name = name;
            PasswordHash = passwordHash;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(Name), Name);
            command.Parameters.AddWithValue(nameof(PasswordHash), 
                Convert.ToBase64String(PasswordHash));
            return command;
        }
    }
}