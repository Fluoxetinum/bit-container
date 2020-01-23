using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Controllers.Ui.EventParams
{
    public class EntityRenamedEventArgs : EventArgs
    {
        public readonly CStorageEntityId EntityId;
        public readonly string NewName;

        public EntityRenamedEventArgs(CStorageEntityId entityId, string newName)
        {
            EntityId = entityId;
            NewName = newName;
        }
    }
}
