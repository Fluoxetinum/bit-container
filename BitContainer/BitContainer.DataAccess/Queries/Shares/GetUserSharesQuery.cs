using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Mappers.Shares;
using BitContainer.DataAccess.Mappers.StorageEntities;

using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.Shares
{
    public class GetUserSharesQuery : AbstractReadDictionaryQuery<CStorageEntityId, EAccessType>
    {
        public CUserId UserId { get; set; }

        private static readonly String QueryString = $"SELECT {DbNames.Shares.PxEntityId} AS {DbNames.Entities.Id}, {DbNames.Shares.PxAccessTypeId} " +
                                                     $"FROM {DbNames.Shares} " +
                                                     $"WHERE {DbNames.Shares.PxUserApprovedId} = @{nameof(UserId)}";


        public GetUserSharesQuery(CUserId userId) : base(new CStorageEntityIdMapper(), new CAccessTypeMapper())
        {
            UserId = userId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(UserId), UserId.ToGuid());
            return command;
        }
    }
}
