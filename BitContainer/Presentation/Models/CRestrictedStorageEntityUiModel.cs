using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Presentation.Models
{
    public class CRestrictedStorageEntityUiModel : IAccessWrapperUiModel
    {
        public IStorageEntityUiModel Entity { get; set; }
        public EAccessTypeUiModel Access { get; set; }

        public CRestrictedStorageEntityUiModel(IStorageEntityUiModel entity, EAccessTypeUiModel access)
        {
            Entity = entity;
            Access = access;
        }
    }

}
