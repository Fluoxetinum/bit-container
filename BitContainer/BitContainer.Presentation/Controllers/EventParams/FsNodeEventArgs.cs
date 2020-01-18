using System;
using System.Collections.Generic;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.Controllers.EventParams
{
    public class FsNodeEventArgs : EventArgs
    {
        public readonly CFileSystemNode Node;

        public FsNodeEventArgs(CFileSystemNode node)
        {
            Node = node;
        }
    }
}
