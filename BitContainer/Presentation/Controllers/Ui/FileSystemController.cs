using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BitContainer.Presentation.Icons;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Enums;

namespace BitContainer.Presentation.Controllers
{
    public class FileSystemController
    {
        //TODO: Get rid of static controllers, program flow becomes increasingly obscure. (Andrey Gurin)

        public CFileSystemNode Root { get; set; } 
        public CFileSystemNode SharedRoot { get; set; }

        private readonly Dictionary<Guid, CFileSystemNode> _ownFs;
        private readonly Dictionary<Guid, CFileSystemNode> _sharedFs;
        
        public FileSystemEventsController FileSystemEvents { get; set; }

        public FileSystemController()
        {
            Root = CFileSystemNode.CreateRootMock(name:"🗃");
            SharedRoot = CFileSystemNode.CreateSharedRootMock(name:"🔗");
            _ownFs = new Dictionary<Guid, CFileSystemNode>()
                {
                    [Root.EntityId] = Root
                };
            _sharedFs = new Dictionary<Guid, CFileSystemNode>()
            {
                [SharedRoot.EntityId] = SharedRoot
            };

            FileSystemEvents = new FileSystemEventsController();
        }

        private void RemoveFromFsModel(CFileSystemNode node)
        {
            if (_ownFs.ContainsKey(node.EntityId))
                _ownFs.Remove(node.EntityId);
            if (_sharedFs.ContainsKey(node.EntityId))
                _sharedFs.Remove(node.EntityId);
        }
        
        public Task<LinkedList<CFileSystemNode>> ComputePath(CFileSystemNode leaf)
        {
            return Task.Run(() =>
            {
                LinkedList<CFileSystemNode> result = new LinkedList<CFileSystemNode>();
                while (leaf != Root && leaf != SharedRoot)
                {
                    result.AddFirst(leaf);
                    leaf = leaf.Parent;
                }
                result.AddFirst(leaf);
                
                return result;
            });
        }

        public async Task FetchChildren(Guid nodeId, Boolean shared)
        {
            CFileSystemNode node;

            if (shared)
                node = _sharedFs[nodeId];
            else
                node = _ownFs[nodeId];

            await FetchChildren(node);
        }

        public async Task FetchChildren(CFileSystemNode node)
        {
            if (node.IsFile) return;
            
            //TODO: Cache (Andrey Gurin)
            //if (node.HasChildren)
            //{
            //    FileSystemEvents.NotifyDirectoryOpened(node);
            //    return;
            //}

            List<IAccessWrapperUiModel> results = null;
            Dictionary<Guid, CFileSystemNode> fs = null;
            switch (node.AccessWrapper)
            {
                case COwnStorageEntityUiModel own:
                    results = await StorageController.GetOwnerStorageEntities(node.EntityId);
                    fs = _ownFs;
                    break;
                case CRestrictedStorageEntityUiModel restricted:
                    results = await StorageController.GetSharedStorageEntities(node.EntityId);
                    fs = _sharedFs;
                    break;
                default:
                    throw new NotSupportedException("Not supported access wrapper.");
            }

            ObservableCollection<CFileSystemNode> nodes = new ObservableCollection<CFileSystemNode>();
            foreach (var r in results)
            {
                CFileSystemNode newNode = new CFileSystemNode(node, r);
                fs[r.Entity.Id] = newNode; 
                nodes.Add(newNode);
            }
            node.Children = nodes;

            FileSystemEvents.NotifyDirectoryOpened(node);
        }

        public async Task CreateDirectory(String name, CFileSystemNode parent)
        {
            if (parent.IsFile) return;
            
            IAccessWrapperUiModel newDir = await StorageController.CreateDirectory(name, parent.EntityId);
            CFileSystemNode newNode = new CFileSystemNode(parent, newDir);
            
            FileSystemEvents.NotifyStorageEntityCreated(newNode);
        }

        public async Task UploadFile(String name, CFileSystemNode parent)
        {
            if (parent.IsFile) return;

            IAccessWrapperUiModel newFIle = await StorageController.UploadFile(name, parent.EntityId);
            CFileSystemNode newNode = new CFileSystemNode(parent, newFIle);

            FileSystemEvents.NotifyStorageEntityCreated(newNode);
        }

        public async Task Delete(CFileSystemNode node)
        {
            RemoveFromFsModel(node);
            
            await node.Entity.Delete();

            FileSystemEvents.NotifyStorageEntityDeleted(node);
        }

        public async Task Rename(CFileSystemNode node, String name)
        {
            await node.Entity.Rename(name);
        }

        public async Task UpdateShare(String userName, EAccessTypeUiModel access, CFileSystemNode node)
        {
            await StorageController.UpdateShare(userName, access, node.Entity);
        }

        public async Task Copy(CFileSystemNode node, CFileSystemNode newParent)
        {
            await node.Entity.Copy(newParent.EntityId);
        }

    }
}
