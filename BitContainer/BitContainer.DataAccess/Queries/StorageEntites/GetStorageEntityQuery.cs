using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Mappers.StorageEntities;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.StorageEntites
{
    public class GetStorageEntityQuery : AbstractScalarQuery<IStorageEntity>
    {
        public CStorageEntityId EntityId { get; }
        
        public CStorageEntityId ParentEntityId { get; }
        public CUserId OwnerId { get; }
        public String Name { get; }

        private static readonly String QueryByIdString =
            $"SELECT {DbNames.Entities.PxId}, {DbNames.Entities.PxParentId}, {DbNames.Entities.PxOwnerId}, " +
            $"{DbNames.Entities.PxName}, {DbNames.Entities.PxCreated}, {DbNames.Entities.PxSize} " +
            $"FROM {DbNames.Entities} WHERE ID = @{nameof(EntityId)}";

        private static readonly String QueryByParamsString =
            $"SELECT {DbNames.Entities.PxId}, {DbNames.Entities.PxParentId}, {DbNames.Entities.PxOwnerId}, " +
            $"{DbNames.Entities.PxName}, {DbNames.Entities.PxCreated}, {DbNames.Entities.PxSize} " +
            $"FROM {DbNames.Entities} " +
            $"WHERE {DbNames.Entities.PxName} = @{nameof(Name)} " +
            $"AND {DbNames.Entities.PxParentId} ";

        private static readonly String QueryByParamsRootEndString = $"IS NULL " +
                                                                    $"AND {DbNames.Entities.PxOwnerId} = @{nameof(OwnerId)} ";
        private static readonly String QueryByParamsEndString = $"= @{nameof(ParentEntityId)};";

        private readonly bool _isQueryById;

        public GetStorageEntityQuery(CStorageEntityId entityId) : base(new CStorageEntityMapper())
        {
            EntityId = entityId;
            OwnerId = new CUserId();
            Name = String.Empty;
            _isQueryById = true;
        }

        public GetStorageEntityQuery(CStorageEntityId parentId, CUserId owner, String name) : base(new CStorageEntityMapper())
        {
            ParentEntityId = parentId;
            OwnerId = owner;
            Name = name;
            _isQueryById = false;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            if (_isQueryById)
                command.CommandText = QueryByIdString;
            else
                command.CommandText = QueryByParamsString + (ParentEntityId.IsRootId ? QueryByParamsRootEndString : QueryByParamsEndString);
            
            command.Parameters.AddWithValue(nameof(EntityId), EntityId.ToGuid());
            command.Parameters.AddWithValue(nameof(ParentEntityId), ParentEntityId.ToGuid());
            command.Parameters.AddWithValue(nameof(OwnerId), OwnerId.ToGuid());
            command.Parameters.AddWithValue(nameof(Name), Name);
            return command;
        }
    }
}
