using System;
using System.Collections.Generic;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels;

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
