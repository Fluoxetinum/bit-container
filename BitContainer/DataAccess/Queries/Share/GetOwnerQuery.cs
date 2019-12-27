using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Share
{
    public class GetOwnerQuery : AbstractScalarQuery<CUser>
    {
        public Guid EntityId { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Users.Id}, {DbNames.Users.Name} " +
            $"FROM {DbNames.Users} JOIN {DbNames.Entities} " +
            $"ON {DbNames.Entities.OwnerId} = {DbNames.Users.Id} " +
            $"WHERE {DbNames.Entities.Id} = @{nameof(EntityId)};";

        public GetOwnerQuery(Guid entityId) : base(new CUserMapper())
        {
            EntityId = entityId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(EntityId), EntityId);
            return command;
        }
    }
}