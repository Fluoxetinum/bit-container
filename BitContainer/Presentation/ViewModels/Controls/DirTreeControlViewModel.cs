using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.EventParams;
using BitContainer.Presentation.Icons;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands.Generic;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.ViewModels.Controls
{
    public class DirTreeControlViewModel : NavigatableViewModelBase, IDisposable
    {
        #region ConnectViewAndViewModelEvents

        public event EventHandler<FsNodeEventArgs> DirectoryOpenedInTreeViewModel;

        public void NotifyDirTreeViewDirectoryOpened(CFileSystemNode directory)
        {
            DirectoryOpenedInTreeViewModel?.Invoke(null, new FsNodeEventArgs(directory));
        }

        #endregion

        private readonly FileSystemController _fileSystemController;

        private readonly DirTreeNode _treeRoot;

        private ObservableCollection<DirTreeNode> _tree;
        public ObservableCollection<DirTreeNode> Tree
        {
            get => _tree;
            set
            {
                _tree = value;
                OnPropertyChanged();
            }
        } 

        private readonly DirTreeNode _sharedTreeRoot;

        private ObservableCollection<DirTreeNode> _sharedTree;
        public ObservableCollection<DirTreeNode> SharedTree 
        {
            get => _sharedTree;
            set
            {
                _sharedTree = value;
                OnPropertyChanged();
            }
        } 

        public DirTreeControlViewModel(FileSystemController filesSystemController)
        {
            _treeRoot = DirTreeNode.Create(filesSystemController.Root);
            _sharedTreeRoot = DirTreeNode.Create(filesSystemController.SharedRoot);
            _treeRoot.Children = DirTreeNode.ChildrenNull;
            _sharedTreeRoot.Children = DirTreeNode.ChildrenNull;
            Tree = new ObservableCollection<DirTreeNode>()
            {
                _treeRoot
            };
            SharedTree = new ObservableCollection<DirTreeNode>()
            {
                _sharedTreeRoot
            };
            _fileSystemController = filesSystemController;

            _fileSystemController.FileSystemEvents.DirectoryOpened += 
                EventsControllerOnDirectoryOpened;
            _fileSystemController.FileSystemEvents.StorageEntityCreated += 
                EventsControllerOnStorageEntityCreated;
            _fileSystemController.FileSystemEvents.StorageEntityDeleted += 
                EventsControllerOnStorageEntityDeleted;
        }

        public async void EventsControllerOnStorageEntityDeleted(object sender, FsNodeEventArgs e)
        {
            if (e.Node.IsFile) return;
            CFileSystemNode fsNode = e.Node;
            DirTreeNode parentNode = await FindDirTreeNode(fsNode.Parent);
            DirTreeNode treeNode = 
                parentNode.Children.SingleOrDefault(n => n.FsNode.EntityId == fsNode.EntityId);
            parentNode.Children.Remove(treeNode);
        }

        public async void EventsControllerOnStorageEntityCreated(object sender, FsNodeEventArgs e)
        {
            if (e.Node.IsFile) return;
            CFileSystemNode fsNode = e.Node;
            DirTreeNode parentNode = await FindDirTreeNode(fsNode.Parent);
            parentNode.Children.Add(DirTreeNode.Create(fsNode));
        }

        public Boolean OpenDirEvent { get; set; } = false;

        public async void EventsControllerOnDirectoryOpened(object sender, FsNodeEventArgs e)
        {
            OpenDirEvent = true;
            CFileSystemNode fsNode = e.Node;

            DirTreeNode node = await FindDirTreeNode(fsNode);

            //TODO: Cache (Andrey Gurin)
            //if (node.Children != DirTreeNode.ChildrenNull)
            //{
            //    if (fsNode.Children.Count > 0)
            //        NotifyDirTreeViewDirectoryOpened(fsNode);
            //    return;
            //};

            node.Children = new ObservableCollection<DirTreeNode>();
            foreach (var child in fsNode.Children)
            {
                if (child.IsFile) continue;
                node.Children.Add(DirTreeNode.Create(child));
            }


            OpenDirEvent = false;
            //if (fsNode.Children.Count > 0)
                NotifyDirTreeViewDirectoryOpened(fsNode);


        }

        private async Task<DirTreeNode> FindDirTreeNode(CFileSystemNode fsNode)
        {
            LinkedList<CFileSystemNode> path = await _fileSystemController.ComputePath(fsNode);

            DirTreeNode node = null;

            switch (fsNode.AccessWrapper)
            {
                case COwnStorageEntityUiModel own:
                    node = _treeRoot;
                    break;
                case CRestrictedStorageEntityUiModel restricted:
                    node = _sharedTreeRoot;
                    break;
                default:
                    throw new NotSupportedException("Not supported access.");
            }
            
            LinkedListNode<CFileSystemNode> pathEntry = path.First;

            while (node.FsNode.EntityId != fsNode.EntityId)
            {
                pathEntry = pathEntry.Next;
                foreach (var child in node.Children)
                {
                    if (child.FsNode.EntityId != pathEntry.Value.EntityId) continue;
                    node = child;
                    break;
                }
            }

            return node;
        }

        private ICommand _directoryExpandedCommand;

        public ICommand DirectoryExpandedCommand =>
            _directoryExpandedCommand ??= new RelayCommand<DirTreeNode>(DirectoryExpanded);

        public async void DirectoryExpanded(DirTreeNode node)
        {
            await _fileSystemController.FetchChildren(node.FsNode);
        }

        public void Dispose()
        {
            _fileSystemController.FileSystemEvents.DirectoryOpened -= EventsControllerOnDirectoryOpened;
            _fileSystemController.FileSystemEvents.StorageEntityCreated -= EventsControllerOnStorageEntityCreated;
            _fileSystemController.FileSystemEvents.StorageEntityDeleted -= EventsControllerOnStorageEntityDeleted;
        }
    }
}
