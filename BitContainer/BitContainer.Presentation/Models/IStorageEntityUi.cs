using System;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Models
{
    public interface IStorageEntityUi
    {
        CStorageEntityId Id { get; set; }
        CStorageEntityId ParentId { get; set; }
        CUserId OwnerId { get; set; }
        String Name {get; set; }
        DateTime Created { get; set; }
    }
}
