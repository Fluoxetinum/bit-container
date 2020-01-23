using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.Service;
using BitContainer.Presentation.Controllers.Ui.EventParams;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Controls;
using BitContainer.Presentation.ViewModels.Nodes;
using Microsoft.Xaml.Behaviors;

namespace BitContainer.Presentation.Views.Controls
{
    /// <summary>
    /// Interaction logic for DirTreeControl.xaml
    /// </summary>
    public partial class DirTreeControl : UserControl
    {
        //TODO: Maybe there is some way to eliminate code behind. (Andrey Gurin)

        public DirTreeControl()
        {
            InitializeComponent();
        }

        private DirTreeControlViewModel _vm;

        private Boolean _muteEvents;

        private void DirTreeControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (CAuthController.CurrentUser == null) return; // Designer exception fix.

            _vm = (DirTreeControlViewModel) this.DataContext;
            _vm.DirectoryOpenedInTreeViewModel += DirectoryOpenedInViewModel;
        }

        private void DirTreeControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _vm.DirectoryOpenedInTreeViewModel -= DirectoryOpenedInViewModel;
        }

        private TreeViewItem GetVisualTreeRoot(FileSystemNode fsNode)
        {
            TreeView treeView = null;
            if (fsNode.IsSharedWithUser)
                treeView = SharedDirsTreeView;
            else
                treeView = DirsTreeView;

            treeView.UpdateLayout();
            return treeView.ItemContainerGenerator.ContainerFromItem(treeView.Items[0])
                as TreeViewItem;
        }

        private List<TreeViewItem> GetVisualTreeChildren(TreeViewItem item)
        {
            List<TreeViewItem> children = new List<TreeViewItem>();

            item.UpdateLayout();
            foreach (var treeViewItem in item.Items)
            {
                object i = item.ItemContainerGenerator.ContainerFromItem(treeViewItem);
                TreeViewItem tvi = i as TreeViewItem;
                if (tvi == null) 
                    throw new ApplicationException("Dir tree is not mapped to virtual file system.");
                children.Add(tvi);
            }

            return children;
        }

        private async Task<TreeViewItem> GetCorrespondingTreeViewItem(FileSystemNode fsNode)
        {
            LinkedList<FileSystemNode> pathFromRoot = await _vm.FileSystemController.GetPathFromRoot(fsNode);
            TreeViewItem treeViewRoot = GetVisualTreeRoot(fsNode);

            List<TreeViewItem> tier = new List<TreeViewItem>() { treeViewRoot };
            TreeViewItem founded = null;

            int tierNumber = 0;
            int pathEntriesNumber = pathFromRoot.Count;
            foreach (var pathEntry in pathFromRoot)
            {
                founded = 
                    tier.SingleOrDefault(item => ((DirTreeNode)item.DataContext).FsNode.Equals(pathEntry));
                if (founded == null) 
                    throw new ApplicationException("Dir tree is not mapped to virtual file system.");
                tier.Clear();
                tierNumber++;
                if (tierNumber < pathEntriesNumber)
                    tier = new List<TreeViewItem>(GetVisualTreeChildren(founded));
            }
            if (founded == null) 
                throw new ApplicationException("Dir tree is not mapped to virtual file system.");

            return founded;
        }

        private async void DirectoryOpenedInViewModel(object sender, NodeOpenedEventArgs e)
        {
            TreeViewItem founded = await GetCorrespondingTreeViewItem(e.Node);
            _muteEvents = true;
            founded.IsSelected = true;
            founded.IsExpanded = true;
            _muteEvents = false;
        }

        private void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
        {
            if (_muteEvents) return;
            TreeViewItem item = (TreeViewItem) e.OriginalSource;
            OpenTreeViewItem(item);
        }

        private void TreeViewItem_OnSelected(object sender, RoutedEventArgs e)
        {
            if (_muteEvents) return;
            TreeViewItem item = (TreeViewItem) e.OriginalSource;
            OpenTreeViewItem(item);
        }

        private void OpenTreeViewItem(TreeViewItem item)
        {
            DirTreeControlViewModel model = (DirTreeControlViewModel) this.DataContext;
            if (model.MuteViewEvents) return;
            model.DirectoryExpandedCommand.Execute(item.DataContext);
        }
    }
}
