using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Share
{
    public class UpdateShareQuery : AbstractWriteQuery
    {
        public Guid PersonApprovedId { get; set; }
        public ERestrictedAccessType RestrictedAccessType { get; set; }
        public Guid EntityId { get; set; }

        private static readonly String QueryString = 
            $"UPDATE {DbNames.Shares} " +
            $"SET {DbNames.Shares.AccessTypeId} = @{nameof(RestrictedAccessType)} " +
            $"WHERE {DbNames.Shares.UserApprovedId} = @{nameof(PersonApprovedId)} " +
            $"AND {DbNames.Shares.EntityId} = @{nameof(EntityId)}";

        public UpdateShareQuery(Guid personApprovedId, ERestrictedAccessType restrictedAccessType, Guid entityId)
        {
            PersonApprovedId = personApprovedId;
            RestrictedAccessType = restrictedAccessType;
            EntityId = entityId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(PersonApprovedId), PersonApprovedId);
            command.Parameters.AddWithValue(nameof(RestrictedAccessType), RestrictedAccessType.ToInt32());
            command.Parameters.AddWithValue(nameof(EntityId), EntityId);
            return command;
        }
    }
}
