using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Models
{
    public class CShareUi
    {
        public CStorageEntityId EntityId { get; set; }
        public CUserId UserId { get; set; }
        public EAccessType Access { get; set; }

        public CShareUi(CStorageEntityId entityId, CUserId userId, EAccessType access)
        {
            EntityId = entityId;
            UserId = userId;
            Access = access;
        }
    }
}
