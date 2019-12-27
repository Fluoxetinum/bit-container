using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Presentation.Models;

namespace BitContainer.Presentation.Icons
{
    public class COwnStorageEntityUiModel : IAccessWrapperUiModel
    {
        public IStorageEntityUiModel Entity { get; set; }
        public Boolean IsShared { get; set; }

        public COwnStorageEntityUiModel(IStorageEntityUiModel entity, Boolean isShared)
        {
            Entity = entity;
            IsShared = isShared;
        }
    }
}
