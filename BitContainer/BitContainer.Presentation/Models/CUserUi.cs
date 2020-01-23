using System;
using BitContainer.Contracts.V1.Auth;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Models
{
    public class CUserUi : ViewModelBase
    {
        public CUserId Id { get; set; }
        public String Token { get; set; }
        public string Name { get; set; }
        
        private Int32 _filesCount;
        public Int32 FilesCount
        {
            get => _filesCount;
            set
            {
                _filesCount = value;
                OnPropertyChanged();
            }
        }

        private Int32 _dirsCount;
        public Int32 DirsCount
        {
            get => _dirsCount;
            set
            {
                _dirsCount = value;
                OnPropertyChanged();
            }
        }

        private Int64 _storageSize;
        public Int64 StorageSize
        {
            get => _storageSize;
            set
            {
                _storageSize = value;
                StorageSizeMb = value / 1024 / 1024;
                OnPropertyChanged();
            }
        }

        private Int64 _storageSizeMb;
        public Int64 StorageSizeMb
        {
            get => _storageSizeMb;
            private set
            {
                _storageSizeMb = value;
                OnPropertyChanged();
            }
        }

        public CUserUi(CUserId userId, String name, Int32 filesCount, Int32 dirsCount, Int64 storageSize, String token)
        {
            Id = userId;
            Name = name;
            FilesCount = filesCount;
            DirsCount = dirsCount;
            StorageSize = storageSize;
            Token = token;
        }
    }
}
