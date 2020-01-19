using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Mappers.Basic;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    public class GetFileStreamPathQuery : AbstractScalarQuery<CFileStreamInfo>
    {
        public Guid Id { get; set; }

        private static readonly String QueryString = $"SELECT {DbNames.Entities.Data}.PathName() AS Path, " +
                                                     $"GET_FILESTREAM_TRANSACTION_CONTEXT() AS TransactionContext " +
                                                     $"FROM StorageEntities WHERE ID = @{nameof(Id)};";

        public GetFileStreamPathQuery(Guid id) : base(new CFileStreamInfoMapper())
        {
            Id = id;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(Id), Id);
            return command;
        }
    }
}
