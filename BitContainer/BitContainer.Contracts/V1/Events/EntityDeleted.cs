using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.Contracts.V1.Events
{
    public class EntityDeleted
    {
        public Guid DeletedEntityId { get; set; }

        public EntityDeleted() {}

        public EntityDeleted(Guid entiyId)
        {
            DeletedEntityId = entiyId;
        }
    }
}
