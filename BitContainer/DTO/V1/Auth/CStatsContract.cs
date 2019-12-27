using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.Auth
{
    public class CStatsContract
    {
        public Int32 FilesCount { get; set; }
        public Int32 DirsCount { get; set; }
        public Int32 StorageSize { get; set; }

        public CStatsContract(Int32 filesCount, Int32 dirsCount, Int32 storageSize)
        {
            FilesCount = filesCount;
            DirsCount = dirsCount;
            StorageSize = storageSize;
        }

    }
}
