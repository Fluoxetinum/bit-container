using System;
using System.Text.RegularExpressions;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Models
{
    public class CFileUi : ViewModelBase, IStorageEntityUi
    {
        public CStorageEntityId Id { get; set; }
        public CStorageEntityId ParentId { get; set; }
        public CUserId OwnerId { get; set; }
        public DateTime Created { get; set; }
        public Int64 Size { get; set; }

        public String Extension => Regex.Match(Name, @"(?=.*)\.[^.]+$").Value;

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

        public CFileUi(
            CStorageEntityId id,
            CStorageEntityId parentId,
            CUserId ownerId,
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
    }
}
