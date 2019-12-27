using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    public class RemoveDirQuery : AbstractScalarQuery<Int32>
    {
        public Guid DirId { get; set; }

        private static readonly String ProcedureName = $"{DbNames.RemoveDir}";

        public RemoveDirQuery(Guid dirId) : base(new CInt32Mapper())
        {
            DirId = dirId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = ProcedureName;
            command.Parameters.AddWithValue(DbNames.RemoveDir.DirId, DirId);
            return command;
        }
    }
}
