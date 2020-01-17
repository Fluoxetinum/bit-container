using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Share
{
    public class GetSharedRootChildrenQuery : AbstractReadQuery<CRestrictedStorageEntity>
    {
        public Guid PersonId { get; set; }

        private readonly String _procedureName = $"{DbNames.GetSharedRootChildren}";

        public GetSharedRootChildrenQuery(Guid person) : base(new CRestrictedDirectoryMapper())
        {
            PersonId = person;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = _procedureName;
            command.Parameters.AddWithValue($"{nameof(PersonId)}", PersonId);
            return command;
        }
    }
}
