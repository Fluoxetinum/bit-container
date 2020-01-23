using System.Collections.ObjectModel;
using System.Windows.Controls;
using BitContainer.Presentation.ViewModels.Base;

namespace BitContainer.Presentation.ViewModels.Nodes
{
    public class DirTreeNode : ViewModelBase
    {
        public TreeViewItem ViewItem { get; set; }


        private FileSystemNode _fsNode;
        public FileSystemNode FsNode
        {
            get => _fsNode;
            set
            {
                _fsNode = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DirTreeNode> _children;
        public ObservableCollection<DirTreeNode> Children
        {
            get => _children;
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        public static ObservableCollection<DirTreeNode> NoChildren = 
            new ObservableCollection<DirTreeNode>() {new DirTreeNode()};

        private DirTreeNode(){}

        public DirTreeNode(FileSystemNode n)
        {
            FsNode = n;
            Children = NoChildren;
        }
    }
}
