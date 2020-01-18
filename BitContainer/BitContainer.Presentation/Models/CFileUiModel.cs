using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Helpers;
using BitContainer.Presentation.ViewModels.Base;

namespace BitContainer.Presentation.Models
{
    public class CFileUiModel : ViewModelBase, IStorageEntityUiModel
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

        public Int64 Size { get; set; }

        public String Extension => Regex.Match(Name, @"(?=.*)\.[^.]+$").Value;

        public CFileUiModel(
            Guid id,
            Guid parentId,
            Guid ownerId,
            String name,
            DateTime created,
            Int64 size)
        {
            Id = id;
            ParentId = parentId;
            OwnerId = ownerId;
            Name = name;
            Created = created;
            Size = size;
        }

        public async Task Delete()
        {
            await StorageController.DeleteFile(this);
        }

        public async Task Rename(string name)
        {
            Name = name;
            await StorageController.RenameFile(this);
        }

        public async Task Copy(Guid newParentId)
        {
            CFileUiModel copy = new CFileUiModel(Id, newParentId, OwnerId, Name, Created, Size);
            await StorageController.CopyFile(copy);
        }
    }


}
