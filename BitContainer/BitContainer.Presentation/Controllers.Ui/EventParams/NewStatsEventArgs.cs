using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.ViewModels.Jobs;

namespace BitContainer.Presentation.Controllers.Ui.EventParams
{
    public class NewStatsEventArgs : EventArgs
    {
        public readonly Int32 FilesCount;
        public readonly Int32 DirsCount;
        public readonly Int64 StorageSize;

        public NewStatsEventArgs(Int32 filesCount, Int32 dirsCount, Int64 storageSize)
        {
            FilesCount = filesCount;
            DirsCount = dirsCount;
            StorageSize = storageSize;
        }
    }
}
