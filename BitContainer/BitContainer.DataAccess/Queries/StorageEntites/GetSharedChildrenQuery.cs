using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers.Shares;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.StorageEntites
{
    public class GetSharedChildrenQuery  : AbstractReadListQuery<CAccessWrapper>
    {
        public CStorageEntityId ParentId { get; set; }
        public CUserId UserId { get; set; }

        public static readonly String QueryStart = $"SELECT {DbNames.Entities.PxParentId}";
        public static readonly String QueryStartForRoot = $"SELECT NULL AS {DbNames.Entities.ParentId}";

        private static readonly String QueryString =
            $", {DbNames.Entities.PxId}, {DbNames.Entities.PxOwnerId}, " +
            $"{DbNames.Entities.PxName}, {DbNames.Entities.PxCreated}, {DbNames.Entities.PxSize}, " +
            $"{DbNames.Shares.PxAccessTypeId} " +
            $"FROM {DbNames.Entities} " +
            $"INNER JOIN {DbNames.Shares} ON {DbNames.Entities.PxId} = {DbNames.Shares.PxEntityId} " +
            $"LEFT OUTER JOIN {DbNames.Shares} AS S2 ON {DbNames.Entities.PxParentId} = S2.EntityID " +
            $"WHERE {DbNames.Shares.PxUserApprovedId} = @{nameof(UserId)} AND ";

        private static readonly String QueryEnd = $"{DbNames.Entities.PxParentId} = @{nameof(ParentId)};";
        private static readonly String QueryEndForRoot = $"S2.EntityID IS NULL;";

        public GetSharedChildrenQuery(CStorageEntityId parentId, CUserId userId) : base(new CAccessWrapperMapper())
        {
            ParentId = parentId;
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = (ParentId.IsRootId ? QueryStartForRoot : QueryStart)
                                  + QueryString
                                  + (ParentId.IsRootId ? QueryEndForRoot : QueryEnd);

            command.Parameters.AddWithValue(nameof(ParentId), ParentId.ToGuid());
            command.Parameters.AddWithValue(nameof(UserId), UserId.ToGuid());

            return command;
        }
    }
}
