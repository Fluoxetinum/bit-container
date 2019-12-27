using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    public class GetDirChildrenQuery : AbstractReadQuery<CChild>
    {
        public Guid DirId { get; set; }

        private static readonly String ProcedureName = $"{DbNames.GetAllDirChildren}";

        public GetDirChildrenQuery(Guid dirId) : base(new CChildrenMapper())
        {
            DirId = dirId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = ProcedureName;
            command.Parameters.AddWithValue(DbNames.GetAllDirChildren.DirId, DirId);
            return command;
        }
    }
}
