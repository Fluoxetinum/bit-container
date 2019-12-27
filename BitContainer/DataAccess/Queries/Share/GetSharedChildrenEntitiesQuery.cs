using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries.Share
{
    public class GetSharedChildrenEntitiesQuery: AbstractReadQuery<CRestrictedStorageEntity>
    {
        public Guid PersonId { get; set; }
        public Guid ParentId { get; set; }
        public ERestrictedAccessType ParrentRestrictedAccess { get; set; }

        private static readonly String _procedureName = $"{DbNames.GetSharedChildren}";

        public GetSharedChildrenEntitiesQuery(Guid personId, Guid parentId, ERestrictedAccessType parrentRestrictedAccess) 
            : base(new CRestrictedStorageEntityMapper())
        {
            PersonId = personId;
            ParentId = parentId;
            ParrentRestrictedAccess = parrentRestrictedAccess;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = _procedureName;
            
            if (!ParentId.IsRootDir())
                command.Parameters.AddWithValue(DbNames.GetSharedChildren.ParentId, ParentId);
            
            command.Parameters.AddWithValue(DbNames.GetSharedChildren.PersonId, PersonId);
            command.Parameters.AddWithValue(DbNames.GetSharedChildren.ParentAccess, 
                ParrentRestrictedAccess.ToString());
            return command;
        }
    }
}
