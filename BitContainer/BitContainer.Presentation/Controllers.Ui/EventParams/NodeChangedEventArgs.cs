using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.Controllers.Ui.EventParams
{
    public class NodeChangedEventArgs : EventArgs
    {
        public readonly FileSystemNode Node;
        public NodeChangedEventArgs(FileSystemNode node)
        {
            Node = node;
        }
    }
}
