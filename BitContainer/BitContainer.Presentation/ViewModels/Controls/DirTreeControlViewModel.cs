using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.Controllers.Ui.EventParams;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands.Generic;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.ViewModels.Controls
{
    public class DirTreeControlViewModel : NavigatableViewModelBase, IDisposable
    {
        #region ConnectViewAndViewModelEvents

        public event EventHandler<NodeOpenedEventArgs> DirectoryOpenedInTreeViewModel;

        public void NotifyDirTreeViewDirectoryOpened(FileSystemNode directory, List<FileSystemNode> children)
        {
            DirectoryOpenedInTreeViewModel?.Invoke(null, new NodeOpenedEventArgs(directory, children));
        }

        public Boolean MuteViewEvents { get; private set; }

        #endregion

        public FileSystemController FileSystemController { get; }

        private readonly DirTreeNode _ownTreeRoot;

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

        private DirTreeNode GetTreeRoot(FileSystemNode node) =>
            node.IsSharedWithUser ? _sharedTreeRoot : _ownTreeRoot;

        public DirTreeControlViewModel(FileSystemController filesSystemController)
        {
            _ownTreeRoot = new DirTreeNode(filesSystemController.Root);
            _sharedTreeRoot = new DirTreeNode(filesSystemController.SharedRoot);

            Tree = new ObservableCollection<DirTreeNode>() { _ownTreeRoot };
            SharedTree = new ObservableCollection<DirTreeNode>() { _sharedTreeRoot };

            FileSystemController = filesSystemController;

            FileSystemController.FileSystemEvents.DirectoryOpened += DirectoryOpened;
            FileSystemController.FileSystemEvents.SearchParentOpened += DirectoryOpened;
            FileSystemController.FileSystemEvents.StorageEntityCreated += EntityCreated;
            FileSystemController.FileSystemEvents.StorageEntityDeleted += EntityDeleted;
        }

        public void Dispose()
        {
            FileSystemController.FileSystemEvents.DirectoryOpened -= DirectoryOpened;
            FileSystemController.FileSystemEvents.SearchParentOpened -= DirectoryOpened;
            FileSystemController.FileSystemEvents.StorageEntityCreated -= EntityCreated;
            FileSystemController.FileSystemEvents.StorageEntityDeleted -= EntityDeleted;
        }

        private async Task<DirTreeNode> GetCorrespondingDirNode(FileSystemNode fsNode)
        {
            LinkedList<FileSystemNode> pathFromRoot = await FileSystemController.GetPathFromRoot(fsNode);

            DirTreeNode treeRoot = GetTreeRoot(fsNode);

            List<DirTreeNode> tier = new List<DirTreeNode>() { treeRoot };

            DirTreeNode founded = null;
            foreach (var pathEntry in pathFromRoot)
            {
                founded = tier.SingleOrDefault(dirNode => dirNode.FsNode.Equals(pathEntry));
                if (founded == null) return null;
                tier.Clear();
                tier = new List<DirTreeNode>(founded.Children);
            }

            return founded;
        }

        #region EventCallbacks

        public async void EntityDeleted(object sender, NodeChangedEventArgs e)
        {
            if (e.Node.IsFile) return;
            FileSystemNode fsNode = e.Node;
            DirTreeNode parentNode = await GetCorrespondingDirNode(fsNode.Parent);

            if (parentNode == null) return;
            
            DirTreeNode treeNode = parentNode.Children.SingleOrDefault(n => n.FsNode.Equals(fsNode));
            parentNode.Children.Remove(treeNode);
        }

        public async void EntityCreated(object sender, NodeChangedEventArgs e)
        {
            if (e.Node.IsFile) return;
            FileSystemNode fsNode = e.Node;
            DirTreeNode parentNode = await GetCorrespondingDirNode(fsNode.Parent);

            if (parentNode == null) return;

            parentNode.Children.Add(new DirTreeNode(fsNode));
        }

        public async void DirectoryOpened(object sender, NodeOpenedEventArgs e)
        {
            MuteViewEvents = true;

            FileSystemNode fsNode = e.Node;
            List<FileSystemNode> children = e.Children;

            DirTreeNode node = await GetCorrespondingDirNode(fsNode);

            node.Children = new ObservableCollection<DirTreeNode>();
            foreach (var child in children)
            {
                if (child.IsFile) continue;
                node.Children.Add(new DirTreeNode(child));
            }

            MuteViewEvents = false;

            NotifyDirTreeViewDirectoryOpened(fsNode, children);
        }

        #endregion

        private ICommand _directoryExpandedCommand;

        public ICommand DirectoryExpandedCommand =>
            _directoryExpandedCommand ??= new RelayCommand<DirTreeNode>(DirectoryExpanded);

        public async void DirectoryExpanded(DirTreeNode node)
        {
            await FileSystemController.OpenDirectory(node.FsNode);
        }
    }
}
