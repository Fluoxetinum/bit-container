using System;

namespace BitContainer.Contracts.V1
{
    public interface IStorageEntityContract
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        Guid OwnerId { get; set; }
        
        String Name { get; set; }
        DateTime Created { get; set; }
    }
}
