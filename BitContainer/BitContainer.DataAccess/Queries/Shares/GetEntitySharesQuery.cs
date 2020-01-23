using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers.Shares;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Shares
{
    public class GetEntitySharesQuery : AbstractReadListQuery<CShare>
    {
        public CStorageEntityId EntityId { get; set; }

        public static readonly String QueryString = $"SELECT {DbNames.Shares.PxEntityId}, {DbNames.Shares.PxAccessTypeId}, " +
                                              $"{DbNames.Shares.PxUserApprovedId} " +
                                              $"FROM {DbNames.Shares} " +
                                              $"WHERE {DbNames.Shares.PxEntityId} = @{nameof(EntityId)}";

        public GetEntitySharesQuery(CStorageEntityId entityId) : base(new CShareMapper())
        {
            EntityId = entityId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(EntityId), EntityId.ToGuid());
            return command;
        }
    }
}
