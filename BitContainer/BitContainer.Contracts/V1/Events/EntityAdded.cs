using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Contracts.V1.Shares;
using BitContainer.Contracts.V1.Storage;

namespace BitContainer.Contracts.V1.Events
{
    public class EntityAdded
    {
        public CSharableEntityContract AddedEntity { get; set; }

        public EntityAdded(){}

        public EntityAdded(CSharableEntityContract addedEntity)
        {
            AddedEntity = addedEntity;
        }
    }
}
