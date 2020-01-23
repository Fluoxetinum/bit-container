using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.Service;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.ViewModels.Nodes
{
    public class FileSystemNode : ViewModelBase, IEquatable<FileSystemNode>
    {
        private FileSystemNode _parent;
        public FileSystemNode Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                OnPropertyChanged();
            }
        }

        private ISharableEntityUi _share;

        public ISharableEntityUi Share
        {
            get => _share;
            set
            {
                _share = value;
                OnPropertyChanged();
            }
        }

        public String Name
        {
            get => Share.Entity.Name;
            set
            {
                Share.Entity.Name = value;
                OnPropertyChanged();
            }
        }

        public CStorageEntityId Id => Share.Entity.Id;

        public DateTime CreatedDate => Share.Entity.Created.Date;
        public DateTime CreatedDateTime => Share.Entity.Created;

        public IStorageEntityUi Entity => Share.Entity;

        public Boolean IsFile => Share.Entity is CFileUi;
        public Boolean IsDir => Share.Entity is CDirectoryUi;
        public Boolean IsMock => Share.Entity.Id.IsRootId;

        public Boolean IsSharedWithUser => Share.Entity.OwnerId != CAuthController.CurrentUser.Id;

        private FileSystemNode() {}

        public FileSystemNode(FileSystemNode parent, ISharableEntityUi value)
        {
            Parent = parent;
            Share = value;
        }

        public static FileSystemNode CreateRootMock(String viewedName, bool shared)
        {
            CDirectoryUi mockDir = new CDirectoryUi(
                new CStorageEntityId(), 
                new CStorageEntityId(), 
                (shared ? new CUserId() : CAuthController.CurrentUser.Id), 
                viewedName, 
                DateTime.MinValue);

            CSharableEntityUi mockSharable = 
                new CSharableEntityUi(mockDir, EAccessType.Write, new List<CShareUi>());

            return new FileSystemNode(null, mockSharable);
        }

        public override bool Equals(object obj)
        {
            if (obj is FileSystemNode other) return Id.Equals(other.Id);
            return false;
        }

        public bool Equals([AllowNull] FileSystemNode other)
        {
            if (other == null) return false;
            return Id.Equals(other.Id);
        }

        public override Int32 GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
