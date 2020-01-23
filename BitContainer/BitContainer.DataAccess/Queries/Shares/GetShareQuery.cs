using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers.Shares;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Shares
{
    public class GetShareQuery : AbstractScalarQuery<CShare>
    {
        public CStorageEntityId EntityId { get; set; }
        public CUserId UserId { get; set; }

        private static readonly String QueryString = $"SELECT {DbNames.Shares.PxEntityId}, {DbNames.Shares.PxAccessTypeId}, " +
                                                       $"{DbNames.Shares.PxUserApprovedId} " +
                                                       $"FROM {DbNames.Shares} " +
                                                       $"WHERE {DbNames.Shares.PxUserApprovedId} = @{nameof(UserId)} AND " +
                                                       $" {DbNames.Shares.PxEntityId} = @{nameof(EntityId)}";

        public GetShareQuery(CUserId userId, CStorageEntityId entityId) : base(new CShareMapper())
        {
            EntityId = entityId;
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(EntityId), EntityId.ToGuid());
            command.Parameters.AddWithValue(nameof(UserId), UserId.ToGuid());
            return command;
        }
    }
}
