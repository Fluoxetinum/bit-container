using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.ActionContracts
{
    public class CRenameEntityContract
    {
        public Guid EntityId { get; set; }
        public String NewName { get; set; }

        public CRenameEntityContract(Guid entityId, String newName)
        {
            EntityId = entityId;
            NewName = newName;
        }
    }
}
