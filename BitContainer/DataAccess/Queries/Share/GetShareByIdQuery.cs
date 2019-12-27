using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Get
{
    public class GetShareByIdQuery : AbstractScalarQuery<CShare>
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }

        private static readonly String ProcedureName = $"{DbNames.GetShareById}";

        public GetShareByIdQuery(Guid id, Guid personId) 
            : base(new CShareMapper())
        {
            Id = id;
            PersonId = personId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = ProcedureName;
            command.Parameters.AddWithValue(DbNames.GetShareById.EntityID, Id);
            command.Parameters.AddWithValue(DbNames.GetShareById.PersonId, PersonId);
            return command;
        }
    }
}
