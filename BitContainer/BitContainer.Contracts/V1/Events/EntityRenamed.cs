using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.Events
{
    public class EntityRenamed
    {
        public Guid EntityId { get; set; }
        public String NewName { get; set; }

        public EntityRenamed(){}

        public EntityRenamed(Guid entiyId, String newName)
        {
            EntityId = entiyId;
            NewName = newName;
        }
    }
}
