using System;
using System.Collections.Generic;
using BitContainer.Presentation.Controllers.EventParams;
using BitContainer.Presentation.Controllers.Ui.EventParams;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.Controllers.Ui
{
    public class FileSystemEventsController
    {
        #region DataTransferEvents

        public event EventHandler<NodeOpenedEventArgs> SearchParentOpened;

        public void NotifySearchParentOpened(FileSystemNode directory, List<FileSystemNode> children)
        {
            SearchParentOpened?.Invoke(null, new NodeOpenedEventArgs(directory, children));
        }

        public event EventHandler<NodeOpenedEventArgs> DirectoryOpened;
        
        public void NotifyDirectoryOpened(FileSystemNode directory, List<FileSystemNode> children)
        {
            DirectoryOpened?.Invoke(null, new NodeOpenedEventArgs(directory, children));
        }

        public event EventHandler<NodeChangedEventArgs> StorageEntityCreated;

        public void NotifyStorageEntityCreated(FileSystemNode entity)
        {
            StorageEntityCreated?.Invoke(null, new NodeChangedEventArgs(entity));
        }

        public event EventHandler<NodeChangedEventArgs> StorageEntityDeleted;

        public void NotifyStorageEntityDeleted(FileSystemNode entity)
        {
            StorageEntityDeleted?.Invoke(null, new NodeChangedEventArgs(entity));
        }
        
        #endregion
    }
}
