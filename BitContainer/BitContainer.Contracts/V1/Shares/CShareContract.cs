using System;
using BitContainer.Shared.Models;

namespace BitContainer.Contracts.V1.Shares
{
    public class CShareContract
    {
        public Guid EntityId { get; set; }
        public Guid UserId { get; set; }
        public EAccessType Access { get; set; }

        public CShareContract(Guid entityId, Guid userId, EAccessType access)
        {
            EntityId = entityId;
            UserId = userId;
            Access = access;
        }
    }
}
