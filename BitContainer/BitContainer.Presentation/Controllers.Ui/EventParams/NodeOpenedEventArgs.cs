using System;
using System.Collections.Generic;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.Controllers.Ui.EventParams
{
    public class NodeOpenedEventArgs : NodeChangedEventArgs
    {
        public readonly List<FileSystemNode> Children;
        public NodeOpenedEventArgs(FileSystemNode node, List<FileSystemNode> children) : base(node)
        {
            Children = children;
        }
    }
}
