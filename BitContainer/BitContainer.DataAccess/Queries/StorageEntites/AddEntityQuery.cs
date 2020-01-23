using System;
using System.Data.SqlClient;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Queries.StorageEntites
{
    class AddEntityQuery : AbstractWriteQuery
    {
        public CStorageEntityId ParentId { get; }
        public CUserId OwnerId { get; }
        public String Name { get; }
        public Byte[] Data { get; }
        public Int64 Size { get; }

        private static readonly String QueryString =
            $"INSERT INTO {DbNames.Entities} " +
            $"({DbNames.Entities.PxOwnerId}, {DbNames.Entities.PxName}, {DbNames.Entities.PxData}, " +
            $"{DbNames.Entities.PxSize}, {DbNames.Entities.PxParentId}) " +
            $"VALUES (@{nameof(OwnerId)}, @{nameof(Name)}, ";

        private static readonly String DataInfoForDirString = "NULL, NULL, ";
        private static readonly String DataInfoForFileString = $" @{nameof(Data)}, @{nameof(Size)}, ";
        private static readonly String EndForRootString = "NULL);";
        private static readonly String EndString = $"@{nameof(ParentId)});";

        private readonly Boolean _isAddingDir;

        public AddEntityQuery(
            CStorageEntityId parentId,
            CUserId ownerId,
            String name,
            Byte[] data)
        {
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Data = data;
            Size = data.LongLength;
            _isAddingDir = false;
        }

        public AddEntityQuery(
            CStorageEntityId parentId,
            CUserId ownerId,
            String name)
        {
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Data = new byte[0];
            Size = 0;
            _isAddingDir = true;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString + (_isAddingDir ? DataInfoForDirString : DataInfoForFileString) 
                                              + (ParentId.IsRootId ? EndForRootString : EndString);
            
            command.Parameters.AddWithValue(nameof(ParentId), ParentId.ToGuid());
            command.Parameters.AddWithValue(nameof(OwnerId), OwnerId.ToGuid());
            command.Parameters.AddWithValue(nameof(Name), Name);
            command.Parameters.AddWithValue(nameof(Data), Data);
            command.Parameters.AddWithValue(nameof(Size), Size);
            return command;
        }
    }
}
