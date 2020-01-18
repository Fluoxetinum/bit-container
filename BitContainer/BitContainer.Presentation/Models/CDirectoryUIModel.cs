using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Helpers;
using BitContainer.Presentation.ViewModels.Base;

namespace BitContainer.Presentation.Models
{
    public class CDirectoryUiModel : ViewModelBase, IStorageEntityUiModel
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid OwnerId { get; set; }
        
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

        public DateTime Created { get; set; }

        public CDirectoryUiModel(Guid id, Guid parentId, Guid ownerId, String name, DateTime created)
        {
            Id = id;
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Created = created;
        }

        public async Task Delete()
        {
            await StorageController.DeleteDirectory(this);
        }

        public async Task Rename(string name)
        {
            Name = name;
            await StorageController.RenameDirectory(this);
        }

        public async Task Copy(Guid newParentId)
        {
            CDirectoryUiModel copy = new CDirectoryUiModel(Id, newParentId, OwnerId, Name, Created);
            await StorageController.CopyDir(copy);
        }
    }
}
