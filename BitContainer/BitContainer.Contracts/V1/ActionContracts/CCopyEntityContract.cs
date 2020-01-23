using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.ActionContracts
{
    public class CCopyEntityContract
    {
        public Guid EntityId;
        public Guid NewParentId;

        public CCopyEntityContract(Guid entityId, Guid newParentId)
        {
            EntityId = entityId;
            NewParentId = newParentId;
        }
    }
}
