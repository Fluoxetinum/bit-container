using System;
using System.Data;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Shares
{
    public class AddShareQuery : AbstractWriteQuery
    {
        public CUserId UserApprovedId { get; set; }
        public EAccessType AccessType { get; set; }
        public CStorageEntityId EntityId { get; set; }
        
        private static readonly String QueryString =
            $"INSERT INTO {DbNames.Shares} " +
            $"({DbNames.Shares.PxUserApprovedId}, " +
            $"{DbNames.Shares.PxAccessTypeId}, " +
            $"{DbNames.Shares.PxEntityId}) " +
            $"VALUES " +
            $"(@{nameof(UserApprovedId)}, " +
            $"@{nameof(AccessType)}, " +
            $"@{nameof(EntityId)})";

        public AddShareQuery(
            CUserId userApprovedId, 
            EAccessType accessType,
            CStorageEntityId entityId)
        {
            UserApprovedId = userApprovedId;
            AccessType = accessType;
            EntityId = entityId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(UserApprovedId), UserApprovedId.ToGuid());
            command.Parameters.Add(nameof(AccessType), SqlDbType.Int).Value = AccessType;
            command.Parameters.AddWithValue(nameof(EntityId), EntityId.ToGuid());
            return command;
        }
    }
}
