using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers.Shares;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.StorageEntites
{
    public class GetOwnerChildrenQuery : AbstractReadListQuery<CAccessWrapper>
    {
        public CStorageEntityId ParentId { get; set; }
        public CUserId UserId { get; set; }

        private static readonly String QueryString =
            $"SELECT {DbNames.Entities.PxId}, {DbNames.Entities.PxParentId}, {DbNames.Entities.PxOwnerId}, " +
            $"{DbNames.Entities.PxName}, {DbNames.Entities.PxCreated}, {DbNames.Entities.PxSize}, " +
            $"1 AS {DbNames.Shares.AccessTypeId} " +
            $"FROM {DbNames.Entities} " +
            $"WHERE {DbNames.Entities.PxOwnerId} = @{nameof(UserId)} AND ";

        private static readonly String QueryEnd = $"{DbNames.Entities.PxParentId} = @{nameof(ParentId)};";
        private static readonly String QueryEndForRoot = $"{DbNames.Entities.PxParentId} IS NULL;";
        
        public GetOwnerChildrenQuery(CStorageEntityId parentId, CUserId userId) : base(new CAccessWrapperMapper())
        {
            ParentId = parentId;
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString + (ParentId.IsRootId ? QueryEndForRoot : QueryEnd);
            
            command.Parameters.AddWithValue(nameof(ParentId), ParentId.ToGuid());
            command.Parameters.AddWithValue(nameof(UserId), UserId.ToGuid());
            return command;
        }
    }
}
