using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Presentation.Models;

namespace BitContainer.Presentation.Controllers.Ui.EventParams
{
    public class EntityAddedEventArgs : EventArgs
    {
        public readonly ISharableEntityUi Share;

        public EntityAddedEventArgs(ISharableEntityUi share)
        {
            Share = share;
        }
    }
}
