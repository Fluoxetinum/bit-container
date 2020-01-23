using System;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Models
{
    public class CDirectoryUi : ViewModelBase, IStorageEntityUi
    {
        public CStorageEntityId Id { get; set; }
        public CStorageEntityId ParentId { get; set; }
        public CUserId OwnerId { get; set; }
        public DateTime Created { get; set; }

        private String _name;
        public String Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public CDirectoryUi(CStorageEntityId id, CStorageEntityId parentId, CUserId ownerId, String name, DateTime created)
        {
            Id = id;
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Created = created;
        }
    }
}
