using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BitContainer.Presentation.Icons;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Base;

namespace BitContainer.Presentation.ViewModels.Nodes
{
    public class CFileSystemNode : ViewModelBase
    {
        private CFileSystemNode _parent;
        public CFileSystemNode Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CFileSystemNode> _children;
        public ObservableCollection<CFileSystemNode> Children
        {
            get => _children;
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        private IAccessWrapperUiModel _accessWrapper;
        public IAccessWrapperUiModel AccessWrapper
        {
            get => _accessWrapper;
            set
            {
                _accessWrapper = value;
                OnPropertyChanged();
            }
        }

        public LinkedList<Guid> GetDownList()
        {
            if (AccessWrapper is CSearchResultUiModelDirtyAdapter adapter)
                return adapter.DownPath;
            else
                return null;
        }

        public Guid EntityId => AccessWrapper.Entity.Id;
        public Boolean IsRoot => EntityId.Equals(Guid.Empty);
        public DateTime CreatedDate => AccessWrapper.Entity.Created.Date;
        public DateTime CreatedTime => AccessWrapper.Entity.Created;

        public IStorageEntityUiModel Entity => AccessWrapper.Entity;
        public Boolean HasChildren => Children != CFileSystemNode.ChildrenNull;
        public void ClearChildren() => Children = CFileSystemNode.ChildrenNull;
        
        public Boolean IsDir => AccessWrapper.Entity is CDirectoryUiModel;
        public CDirectoryUiModel Dir => AccessWrapper.Entity as CDirectoryUiModel;
        public Boolean IsFile => AccessWrapper.Entity is CFileUiModel;
        public CFileUiModel File => AccessWrapper.Entity as CFileUiModel;

        private static ObservableCollection<CFileSystemNode> _childrenNull;

        public static ObservableCollection<CFileSystemNode> ChildrenNull =>
            _childrenNull ??= new ObservableCollection<CFileSystemNode>()
                {new CFileSystemNode()};

        private CFileSystemNode() {}

        public CFileSystemNode(CFileSystemNode parent, IAccessWrapperUiModel value, List<CFileSystemNode> children)
        {
            Parent = parent;
            AccessWrapper = value;
            Children = new ObservableCollection<CFileSystemNode>(children);
        }

        public CFileSystemNode(CFileSystemNode parent, IAccessWrapperUiModel value)
        {
            Parent = parent;
            AccessWrapper = value;
            Children = ChildrenNull;
        }

        public static CFileSystemNode CreateRootMock(String name)
        {
            CDirectoryUiModel mockDir =
                new CDirectoryUiModel(
                    Guid.Empty, 
                    Guid.Empty, 
                    Guid.Empty, 
                    name, 
                    DateTime.MinValue);

            COwnStorageEntityUiModel mockOwnModel =
                new COwnStorageEntityUiModel(mockDir, false);

            return new CFileSystemNode()
            {
                Parent = null,
                AccessWrapper = mockOwnModel,
                Children = ChildrenNull
            };
        }

        public static CFileSystemNode CreateSharedRootMock(String name)
        {
            CDirectoryUiModel mockDir =
                new CDirectoryUiModel(
                    Guid.Empty, 
                    Guid.Empty, 
                    Guid.Empty, 
                    name, 
                    DateTime.MinValue);

            CRestrictedStorageEntityUiModel mockRestrictedModel =
                new CRestrictedStorageEntityUiModel(mockDir, EAccessTypeUiModel.Write);

            return new CFileSystemNode()
            {
                Parent = null,
                AccessWrapper = mockRestrictedModel,
                Children = ChildrenNull
            };
        }
    }
}
