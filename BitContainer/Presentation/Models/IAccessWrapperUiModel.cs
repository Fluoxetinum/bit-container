using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Presentation.Models
{
    public interface IAccessWrapperUiModel
    {
        IStorageEntityUiModel Entity { get; set; }
    }
}
