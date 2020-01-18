using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Presentation.Models
{
    public class CSearchResultUiModelDirtyAdapter : IAccessWrapperUiModel
    {
        public LinkedList<Guid> DownPath { get; set; }

        public IAccessWrapperUiModel AccessWrapper { get; set; }

        public IStorageEntityUiModel Entity
        {
            get => AccessWrapper.Entity;
            set => AccessWrapper.Entity = value;
        }

        public CSearchResultUiModelDirtyAdapter(IAccessWrapperUiModel accessWrapper, LinkedList<Guid> downPath)
        {
            AccessWrapper = accessWrapper;
            DownPath = downPath;
        }

    }
}
