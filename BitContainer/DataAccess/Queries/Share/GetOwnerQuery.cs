using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Share
{
    public class GetOwnerQuery : AbstractScalarQuery<Guid>
    {
        public Guid EntityId { get; set; }

        private static readonly String QueryString = 
            $"SELECT {DbNames.Entities.OwnerId} " +
            $"FROM {DbNames.Entities} " +
            $"WHERE {DbNames.Entities.Id} = @{nameof(EntityId)};";

        public GetOwnerQuery(Guid entityId) : base(new CGuidMapper())
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