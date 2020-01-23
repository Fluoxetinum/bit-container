using System;

namespace BitContainer.Contracts.V1.Storage
{
    public class CStatsContract
    {
        public Int32 FilesCount { get; set; }
        public Int32 DirsCount { get; set; }
        public Int64 StorageSize { get; set; }

        public CStatsContract() {}

        public CStatsContract(Int32 filesCount, Int32 dirsCount, Int64 storageSize)
        {
            FilesCount = filesCount;
            DirsCount = dirsCount;
            StorageSize = storageSize;
        }
    }
}
