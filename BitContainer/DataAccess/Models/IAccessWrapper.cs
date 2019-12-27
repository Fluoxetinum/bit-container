using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Models
{
    public interface IAccessWrapper
    {
        IStorageEntity Entity { get; }
    }
}
