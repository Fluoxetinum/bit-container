using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BitContainer.Presentation.Controllers.Service;
using BitContainer.Presentation.Controllers.Ui.EventParams;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Nodes;
using BitContainer.Shared.Models;

namespace BitContainer.Presentation.Controllers.Ui
{
    public class FileSystemController
    {
        private readonly CStorageController _storageController;

        private readonly Dictionary<CStorageEntityId, FileSystemNode> _ownFs;
        private readonly Dictionary<CStorageEntityId, FileSystemNode> _sharedFs;
        
        public FileSystemNode Root { get; set; } 
        public FileSystemNode SharedRoot { get; set; }
        public FileSystemEventsController FileSystemEvents { get; set; }

        public FileSystemController(CStorageController storageController, FileSystemEventsController eventsController)
        {
            _storageController = storageController;
            
            FileSystemEvents = eventsController;

            Root = FileSystemNode.CreateRootMock(viewedName:"🗃", shared:false);
            SharedRoot = FileSystemNode.CreateRootMock(viewedName:"🔗", shared:true);

            _ownFs = new Dictionary<CStorageEntityId, FileSystemNode>()
            {
                [Root.Id] = Root
            };
            _sharedFs = new Dictionary<CStorageEntityId, FileSystemNode>()
            {
                [SharedRoot.Id] = SharedRoot
            };

            FileSystemEvents.EntityRenamed += OnEntityRenamed;
            FileSystemEvents.EntityDeleted += EntityDeleted;
            FileSystemEvents.EntityAdded += OnEntityAdded;
            FileSystemEvents.Connect();
        }

        private void OnEntityAdded(object sender, EntityAddedEventArgs e)
        {
            ISharableEntityUi newEntity = e.Share;

            var fs = GetFsById(newEntity.Entity.ParentId);
            if (fs == null) return;

            FileSystemNode parent = fs[newEntity.Entity.ParentId];
            FileSystemNode newNode = new FileSystemNode(parent, newEntity);
            fs[newEntity.Entity.Id] = newNode;
            FileSystemEvents.NotifyStorageEntityCreated(newNode);
        }

        private void EntityDeleted(object sender, EntityDeletedEventArgs e)
        {
            var fs = GetFsById(e.EntityId);
            if (fs == null) return;

            FileSystemNode node = fs[e.EntityId];
            fs.Remove(e.EntityId);
            FileSystemEvents.NotifyStorageEntityDeleted(node);
        }

        private void OnEntityRenamed(object? sender, EntityRenamedEventArgs e)
        {
            var fs = GetFsById(e.EntityId);
            if (fs == null) return;
            fs[e.EntityId].Name = e.NewName;
        }

        private void RemoveFromFs(FileSystemNode node) => GetFs(node).Remove(node.Id);

        private Dictionary<CStorageEntityId, FileSystemNode> GetFsById(CStorageEntityId id)
        {
            if (_ownFs.ContainsKey(id))
                return _ownFs;
            if (_sharedFs.ContainsKey(id))
                return _sharedFs;
            return null;
        }

        private Dictionary<CStorageEntityId, FileSystemNode> GetFs(FileSystemNode node) => 
            node.IsSharedWithUser ? _sharedFs : _ownFs;

        private List<FileSystemNode> AddToFs(List<ISharableEntityUi> entities, FileSystemNode parent)
        {
            List<FileSystemNode> newNodes = new List<FileSystemNode>();

            Dictionary<CStorageEntityId, FileSystemNode> fs = GetFs(parent);

            foreach (var entity in entities)
            {
                var node = new FileSystemNode(parent, entity);
                fs[entity.Entity.Id] = node;
                newNodes.Add(node);
            }

            return newNodes;
        }
        
        private async Task<List<FileSystemNode>> FetchChildren(FileSystemNode node)
        {
            if (node.IsFile) throw new InvalidOperationException("File cannot be opened.");
            
            List<ISharableEntityUi> results;

            if (node.IsSharedWithUser)
                results = await _storageController.GetSharedStorageEntities(node.Id);
            else
                results = await _storageController.GetOwnerStorageEntities(node.Id);
            
            return AddToFs(results, node);
        }

        private async Task OpenSearchResult(CSearchResultUi searchResult, FileSystemNode node)
        {
            Dictionary<CStorageEntityId, FileSystemNode> fs = GetFs(node);
            List<CStorageEntityId> parents = searchResult.Parents;

            int lastI = parents.Count - 1;
            for (int i = 0; i < lastI; i++)
            {
                FileSystemNode fsNode = fs[parents[i]];
                List<FileSystemNode> result = await FetchChildren(fsNode);
                FileSystemEvents.NotifySearchParentOpened(fsNode, result);
            }

            FileSystemNode lastNode = fs[parents[lastI]];
            List<FileSystemNode> lastResult = await FetchChildren(lastNode);
            FileSystemEvents.NotifyDirectoryOpened(lastNode, lastResult);
        }

        public async Task OpenDirectory(FileSystemNode node)
        {
            if (node.IsFile) return;

            switch (node.Share)
            {
                case CSharableEntityUi share:
                    List<FileSystemNode> children = await FetchChildren(node);
                    FileSystemEvents.NotifyDirectoryOpened(node, children);
                    break;
                case CSearchResultUi searchResult:
                    await OpenSearchResult(searchResult, node);
                    break;
                default:
                    throw new InvalidCastException(nameof(node.Share));
            }
        }

        public async Task Search(FileSystemNode node, String pattern)
        {
            List<ISharableEntityUi> results;

            if (pattern == string.Empty)
            {
                await OpenDirectory(node);
                return;
            }

            if (node.IsSharedWithUser)
                results = await _storageController.SearchSharedEntities(node.Id, pattern);
            else
                results = await _storageController.SearchOwnerEntities(node.Id, pattern);

            List<FileSystemNode> mockNodes = results.Select(r => new FileSystemNode(null, r)).ToList();
            
            FileSystemEvents.NotifyDirectoryOpened(node, mockNodes);
        }

        public async Task DownloadEntity(String filePath, FileSystemNode node)
        {
            await _storageController.LoadEntity(filePath, node.Share.Entity);
        }
        
        public async Task CreateDirectory(String name, FileSystemNode parent)
        {
            if (parent.IsFile) throw new InvalidOperationException("File can only be uploaded from disk by now.");

            ISharableEntityUi newDir = await _storageController.CreateDirectory(name, parent.Id);
            if (newDir == null) return;
            
            FileSystemNode newNode = new FileSystemNode(parent, newDir);
            
            FileSystemEvents.NotifyStorageEntityCreated(newNode);
        }

        public async Task UploadFile(String name, FileSystemNode parent)
        {
            if (parent.IsFile) throw new InvalidOperationException("File can only be uploaded in dir.");

            ISharableEntityUi newFIle = await _storageController.UploadFile(name, parent.Id);
            if (newFIle == null) return;

            FileSystemNode newNode = new FileSystemNode(parent, newFIle);

            FileSystemEvents.NotifyStorageEntityCreated(newNode);
        }

        public async Task Delete(FileSystemNode node)
        {
            RemoveFromFs(node);
            await _storageController.DeleteEntity(node.Id);
            FileSystemEvents.NotifyStorageEntityDeleted(node);
        }

        public async Task Rename(FileSystemNode node, String name)
        {
            node.Name = name;
            await _storageController.RenameEntity(node.Id, name);
        }

        public async Task UpdateShare(String userName, EAccessType access, FileSystemNode node)
        {
            await _storageController.UpdateShare(userName, access, node.Share.Entity);
        }

        public async Task Copy(FileSystemNode node, FileSystemNode newParent)
        {
            await _storageController.CopyEntity(node.Id, newParent.Id);
        }

        public Task<LinkedList<FileSystemNode>> GetPathFromRoot(FileSystemNode leaf)
        {
            return Task.Run(() =>
            {
                var result = new LinkedList<FileSystemNode>();
                while (!leaf.IsMock)
                {
                    result.AddFirst(leaf);
                    leaf = leaf.Parent;
                }
                result.AddFirst(leaf);
                return result;
            });
        }
    }
}
