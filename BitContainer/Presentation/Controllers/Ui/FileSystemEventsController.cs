using System;
using System.Collections.Generic;
using BitContainer.Presentation.Controllers.EventParams;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Jobs;

namespace BitContainer.Presentation.Controllers
{
    public class FileSystemEventsController
    {
        #region DataTransferEvents

        public event EventHandler<FsNodeEventArgs> DirectoryOpened;

        public void NotifyDirectoryOpened(CFileSystemNode directory)
        {
            DirectoryOpened?.Invoke(null, new FsNodeEventArgs(directory));
        }

        public event EventHandler<FsNodeEventArgs> StorageEntityCreated;

        public void NotifyStorageEntityCreated(CFileSystemNode entity)
        {
            StorageEntityCreated?.Invoke(null, new FsNodeEventArgs(entity));
        }

        public event EventHandler<FsNodeEventArgs> StorageEntityDeleted;

        public void NotifyStorageEntityDeleted(CFileSystemNode entity)
        {
            StorageEntityDeleted?.Invoke(null, new FsNodeEventArgs(entity));
        }
        
        #endregion
    }
}
