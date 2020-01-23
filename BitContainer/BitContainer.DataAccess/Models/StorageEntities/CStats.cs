using System;
using BitContainer.Shared.Models;
using Microsoft.SqlServer.Management.Common;

namespace BitContainer.DataAccess.Models.StorageEntities
{
    public class CStats
    {
        public CUserId UserId { get; }
        public Int32 FilesCount { get; private set; }
        public Int32 DirsCount { get; private set; }
        public Int64 StorageSize { get; private set; }

        public CStats(CUserId userId, Int32 filesCount, Int32 dirsCount, Int64 storageSize)
        {
            UserId = userId;
            FilesCount = filesCount;
            DirsCount = dirsCount;
            StorageSize = storageSize;
        }



        public void DeleteUpdate(IStorageEntity deletedEntity)
        {
            switch (deletedEntity)
            {
                case CFile file:
                    FilesCount--;
                    StorageSize -= file.Size;
                    break;
                case CDirectory dir:
                    DirsCount--;
                    break;
                default:
                    throw new InvalidCastException(nameof(deletedEntity));
            }
        }

        public void AddUpdate(IStorageEntity addedEntity)
        {
            switch (addedEntity)
            {
                case CFile file:
                    FilesCount++;
                    StorageSize += file.Size;
                    break;
                case CDirectory dir:
                    DirsCount++;
                    break;
                default:
                    throw new InvalidCastException(nameof(addedEntity));
            }
        }

        public void AddUpdate(Int64 size)
        {
            StorageSize += size;
        }

    }
}
