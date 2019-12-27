using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.Shares
{
    public interface IAccessWrapperContract
    {
        IStorageEntityContract EntityContract { get; set; }
    }
}
