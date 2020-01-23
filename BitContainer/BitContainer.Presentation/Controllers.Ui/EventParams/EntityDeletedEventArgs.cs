using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Controllers.Ui.EventParams
{
    public class EntityDeletedEventArgs : EventArgs
    {
        public readonly CStorageEntityId EntityId;

        public EntityDeletedEventArgs(CStorageEntityId entityId)
        {
            EntityId = entityId;
        }
    }
}
