﻿using System.Collections.ObjectModel;
using BitContainer.Presentation.ViewModels.Base;

namespace BitContainer.Presentation.ViewModels.Nodes
{
    public class DirTreeNode : ViewModelBase
    {
        private CFileSystemNode _fsNode;
        public CFileSystemNode FsNode
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

        private static ObservableCollection<DirTreeNode> _childrenNull;
        public static ObservableCollection<DirTreeNode> ChildrenNull =>
            _childrenNull ??= new ObservableCollection<DirTreeNode>()
                {new DirTreeNode()};

        public static DirTreeNode Create(CFileSystemNode n)
        {
            return new DirTreeNode()
            {
                FsNode = n,
                Children = ChildrenNull
            };
        }
    }
}