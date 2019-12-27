using System;
using BitContainer.Contracts.V1;
using BitContainer.Presentation.ViewModels.Base;

namespace BitContainer.Presentation.Model
{
    public class CUserUiModel : ViewModelBase
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        
        public Int32 FilesCount { get; set; }
        public Int32 DirsCount { get; set; }
        
        public Int32 StorageSize { get; set; }
        public Int32 StorageSizeMb => StorageSize / 1024 / 1024;

        public static CUserUiModel Create(CAuthenticatedUserContract contract)
        {
            return new CUserUiModel()
            {
                Id = contract.User.Id,
                Name = contract.User.Name,
                FilesCount = contract.Stats.FilesCount,
                DirsCount = contract.Stats.DirsCount,
                StorageSize = contract.Stats.StorageSize
            };
        }
    }
}
