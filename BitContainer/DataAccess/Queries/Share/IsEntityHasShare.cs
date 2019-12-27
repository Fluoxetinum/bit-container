using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Share
{
    public class IsEntityHasShare: AbstractScalarQuery<Boolean>
    {
        public Guid EntityId { get; set; }

        private static readonly String QueryString = 
            $"SELECT COUNT({DbNames.Shares.EntityId}) FROM {DbNames.Shares} " +
            $"WHERE {DbNames.Shares.EntityId} = @{nameof(EntityId)};";

        public IsEntityHasShare(Guid entityId) : base(new CBooleanMapper())
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
