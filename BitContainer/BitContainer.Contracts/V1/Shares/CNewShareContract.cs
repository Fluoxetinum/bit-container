using System;
using BitContainer.Shared.Models;

namespace BitContainer.Contracts.V1.Shares
{
    public class CNewShareContract
    {
        public Guid EntityId { get; set; }
        public String UserName { get; set; } 
        public EAccessType AccessType { get; set; }

        public CNewShareContract(String userName, EAccessType access, Guid entityId)
        {
            AccessType = access;
            UserName = userName;
            EntityId = entityId;
        }
    }
}
