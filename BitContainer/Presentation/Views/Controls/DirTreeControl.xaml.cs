using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.EventParams;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Controls;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.Views.Controls
{
    /// <summary>
    /// Interaction logic for DirTreeControl.xaml
    /// </summary>
    public partial class DirTreeControl : UserControl
    {
        //TODO: Maybe there is some way to eliminate code behind. (Andrey Gurin)

        private DirTreeControlViewModel _vm;
        private readonly LinkedList<TreeViewItem> _lastExpandedPath = new LinkedList<TreeViewItem>();
        private Boolean OpenDirEvent = false;


        public DirTreeControl()
        {
            InitializeComponent();
        }

        private void DirTreeControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (AuthController.AuthenticatedUserUiModel == null) return; // Designer exception fix

            _vm = (DirTreeControlViewModel) this.DataContext;
            _vm.DirectoryOpenedInTreeViewModel += EventsControllerDirectoryOpened;

            DirsTreeView.UpdateLayout();
            TreeViewItem root =
                DirsTreeView.ItemContainerGenerator.ContainerFromItem(DirsTreeView.Items[0])
                    as TreeViewItem;
            _lastExpandedPath.Clear();
            _lastExpandedPath.AddFirst(root);
        }
        
        private void UpdateLastExpandedPath(TreeViewItem item)
        {
            _lastExpandedPath.Clear();
            _lastExpandedPath.AddFirst(item);

            DependencyObject obj = VisualTreeHelper.GetParent(item);
            while (!(obj is TreeView))
            {
                obj = VisualTreeHelper.GetParent(obj);
                if (obj == null) break;
                if (obj is TreeViewItem parentItem)
                {
                    _lastExpandedPath.AddFirst(parentItem);
                }
            }
        }

        private List<TreeViewItem> GetChildren(TreeViewItem item)
        {
            List<TreeViewItem> children = new List<TreeViewItem>();

            DirTreeNode node = (DirTreeNode) item.DataContext;

            if (!node.FsNode.HasChildren) return children;

            item.UpdateLayout();
            foreach (var treeViewItem in item.Items)
            {
                object i = item.ItemContainerGenerator.ContainerFromItem(treeViewItem);
                TreeViewItem tvi = i as TreeViewItem;

                if (tvi == null) throw new NotSupportedException("Error");

                children.Add(tvi);
            }

            return children;
        }
        
        private void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
        {
            if (OpenDirEvent) return;
            TreeViewItem item = (TreeViewItem) e.OriginalSource;
            OpenTreeViewItem(item);
        }

        private void TreeViewItem_OnSelected(object sender, RoutedEventArgs e)
        {
            if (OpenDirEvent) return;
            TreeViewItem item = (TreeViewItem) e.OriginalSource;
            OpenTreeViewItem(item);
        }

        private void OpenTreeViewItem(TreeViewItem item)
        {
            UpdateLastExpandedPath(item);
            DirTreeControlViewModel model = (DirTreeControlViewModel) this.DataContext;

            if (model.OpenDirEvent) return;

            model.DirectoryExpandedCommand.Execute(item.DataContext);
        }

        private void EventsControllerDirectoryOpened(object sender, FsNodeEventArgs e)
        {
            TreeViewItem lastExpanded = _lastExpandedPath.Last.Value;

            CFileSystemNode fsNode = e.Node;

            if (SelectTreeViewItemIfMatches(lastExpanded, fsNode)) return;
            
            List<TreeViewItem> lastExpandedChildren = GetChildren(lastExpanded);
            Queue<TreeViewItem> children = new Queue<TreeViewItem>(lastExpandedChildren);

            while (children.Count > 0)
            {
                var lastExpandedChild = children.Dequeue();
                if (SelectTreeViewItemIfMatches(lastExpandedChild, fsNode))
                {
                    return;
                }
                else
                {   
                     GetChildren(lastExpandedChild).ForEach(item => children.Enqueue(item));
                }
            }

            LinkedListNode<TreeViewItem> parent = _lastExpandedPath.Last.Previous;
            while (parent != null)
            {
                TreeViewItem parentItem = parent.Value;
                if (SelectTreeViewItemIfMatches(parentItem, fsNode)) return;
                parent = parent.Previous;
            }
        }

        private Boolean SelectTreeViewItemIfMatches(TreeViewItem item, CFileSystemNode node)
        {
            if (item == null) return false;

            DirTreeNode itemNode = (DirTreeNode) item.DataContext;

            if (node.EntityId != itemNode.FsNode.EntityId) return false;
            OpenDirEvent = true;
            item.IsSelected = true;
            item.IsExpanded = true;
            OpenDirEvent = false;
            return true;
        }

        private void DirTreeControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _vm.DirectoryOpenedInTreeViewModel -= EventsControllerDirectoryOpened;
        }
    }
}
