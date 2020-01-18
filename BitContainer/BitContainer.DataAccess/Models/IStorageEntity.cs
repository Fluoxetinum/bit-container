using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Models
{
    public interface IStorageEntity
    {
        Guid Id { get; }
        Guid ParentId { get;  }
        Guid OwnerId { get;  }
        String Name { get;  }
        DateTime Created { get; }
    } 
}
