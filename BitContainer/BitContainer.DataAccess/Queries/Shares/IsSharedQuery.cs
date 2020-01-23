using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Shares
{
    public class IsSharedQuery: AbstractScalarQuery<Boolean>
    {
        public CStorageEntityId EntityId { get; set; }

        private static readonly String QueryString = 
            $"SELECT COUNT({DbNames.Shares.PxEntityId}) FROM {DbNames.Shares} " +
            $"WHERE {DbNames.Shares.PxEntityId} = @{nameof(EntityId)};";

        public IsSharedQuery(CStorageEntityId entityId) : base(new CMoreThanZeroMapper())
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
