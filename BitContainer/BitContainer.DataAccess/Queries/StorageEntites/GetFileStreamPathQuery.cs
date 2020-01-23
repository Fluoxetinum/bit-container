using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers.StorageEntities;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.StorageEntites
{
    public class GetFileStreamPathQuery : AbstractScalarQuery<CFileStreamInfo>
    {
        public CStorageEntityId Id { get; set; }

        private static readonly String QueryString = $"SELECT {DbNames.Entities.PxData}.PathName() AS Path, " +
                                                     $"GET_FILESTREAM_TRANSACTION_CONTEXT() AS TransactionContext " +
                                                     $"FROM {DbNames.Entities} WHERE ID = @{nameof(Id)};";

        public GetFileStreamPathQuery(CStorageEntityId id) : base(new CFileStreamInfoMapper())
        {
            Id = id;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(Id), Id.ToGuid());
            return command;
        }
    }
}
