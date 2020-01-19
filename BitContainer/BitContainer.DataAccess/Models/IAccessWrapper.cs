using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess.Models.StorageEntities;

namespace BitContainer.DataAccess.Models
{
    public interface IAccessWrapper
    {
        IStorageEntity Entity { get; }
    }
}
