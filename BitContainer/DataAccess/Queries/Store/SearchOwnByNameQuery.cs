using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Store
{
    public class SearchOwnByNameQuery : AbstractReadQuery<COwnStorageEntity>
    {
        public Guid UserId { get; set; }
        public Guid ParentId { get; set; }
        public String Pattern { get; set; }

        public static readonly String ProcedureName = $"{DbNames.SearchOwnByName}";

        public SearchOwnByNameQuery(Guid userId, Guid parentId, String pattern) : base(new COwnStorageEntityMapper())
        {
            UserId = userId;
            ParentId = parentId;
            Pattern = pattern;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = ProcedureName;

            command.Parameters.AddWithValue(DbNames.SearchOwnByName.UserId, UserId);
            command.Parameters.AddWithValue(DbNames.SearchOwnByName.Pattern, Pattern);

            if (!ParentId.IsRootDir()) 
                command.Parameters.AddWithValue(DbNames.SearchOwnByName.ParentId, ParentId);

            return command;
        }
    }
}
