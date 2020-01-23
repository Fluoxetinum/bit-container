using System;
using System.Data;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Shares
{
    public class UpdateShareQuery : AbstractWriteQuery
    {
        public CUserId PersonApprovedId { get; set; }
        public EAccessType AccessType { get; set; }
        public CStorageEntityId EntityId { get; set; }

        private static readonly String QueryString = 
            $"UPDATE {DbNames.Shares} " +
            $"SET {DbNames.Shares.PxAccessTypeId} = @{nameof(AccessType)} " +
            $"WHERE {DbNames.Shares.PxUserApprovedId} = @{nameof(PersonApprovedId)} " +
            $"AND {DbNames.Shares.PxEntityId} = @{nameof(EntityId)}";

        public UpdateShareQuery(CUserId personApprovedId, EAccessType accessType, CStorageEntityId entityId)
        {
            PersonApprovedId = personApprovedId;
            AccessType = accessType;
            EntityId = entityId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(PersonApprovedId), PersonApprovedId.ToGuid());
            command.Parameters.Add(nameof(AccessType), SqlDbType.Int).Value = AccessType;
            command.Parameters.AddWithValue(nameof(EntityId), EntityId.ToGuid());
            return command;
        }
    }
}
