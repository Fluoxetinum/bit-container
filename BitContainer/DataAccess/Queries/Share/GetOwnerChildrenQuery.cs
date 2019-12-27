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
    public class GetOwnerChildrenQuery : AbstractReadQuery<COwnStorageEntity>
    {
        public Guid ParentId { get; set; }
        public Guid OwnerId { get; set; }

        private static readonly String ProcedureName = $"{DbNames.GetOwnerDirChildren}";

        public GetOwnerChildrenQuery(Guid parentId, Guid ownerId) : base(new COwnStorageEntityMapper())
        {
            ParentId = parentId;
            OwnerId = ownerId;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = ProcedureName;
            
            if (!ParentId.IsRootDir())
                command.Parameters.AddWithValue(DbNames.GetOwnerDirChildren.ParentId, ParentId);

            command.Parameters.AddWithValue(DbNames.GetOwnerDirChildren.OwnerId, OwnerId);
            return command;
        }
    }
}
