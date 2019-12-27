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
    public class SearchSharedByNameQuery : AbstractReadQuery<CRestrictedStorageEntity>
    {
        public Guid UserId { get; set; }
        public Guid ParentId { get; set; }
        public String Pattern { get; set; }
        public ERestrictedAccessType ParentAccess { get; set; }

        public static readonly String ProcedureName = $"{DbNames.SearchSharedByName}";

        public SearchSharedByNameQuery(Guid userId, Guid parentId, String pattern, ERestrictedAccessType parentAccess) : base(new CRestrictedStorageEntityMapper())
        {
            UserId = userId;
            ParentId = parentId;
            Pattern = pattern;
            ParentAccess = parentAccess;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = ProcedureName;

            command.Parameters.AddWithValue(DbNames.SearchSharedByName.ParentAccess, ParentAccess);
            command.Parameters.AddWithValue(DbNames.SearchSharedByName.UserId, UserId);
            command.Parameters.AddWithValue(DbNames.SearchSharedByName.Pattern, Pattern);
            
            command.Parameters.AddWithValue(DbNames.SearchSharedByName.ParentId, ParentId);

            return command;
        }
    }
}
