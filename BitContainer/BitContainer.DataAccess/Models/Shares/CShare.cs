using System;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.Models.Shares
{
    public class CShare
    {
        public CUserId UserId { get; }
        public CStorageEntityId EntityId { get; }
        public EAccessType AccessType { get; }

        public CShare(CUserId userId, CStorageEntityId entityId, EAccessType accessType)
        {
            UserId = userId;
            EntityId = entityId;
            AccessType = accessType;
        }
    }
}
