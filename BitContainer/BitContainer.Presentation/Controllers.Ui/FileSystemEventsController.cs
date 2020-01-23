using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using BitContainer.Contracts.V1.Events;
using BitContainer.Contracts.V1.Storage;
using BitContainer.Presentation.Controllers.Service;
using BitContainer.Presentation.Controllers.Ui.EventParams;
using BitContainer.Presentation.Helpers;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Nodes;
using BitContainer.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace BitContainer.Presentation.Controllers.Ui
{
    public class FileSystemEventsController
    {
        #region DataTransferEvents

        public event EventHandler<NodeOpenedEventArgs> SearchParentOpened;

        public void NotifySearchParentOpened(FileSystemNode directory, List<FileSystemNode> children)
        {
            SearchParentOpened?.Invoke(null, new NodeOpenedEventArgs(directory, children));
        }

        public event EventHandler<NodeOpenedEventArgs> DirectoryOpened;

        public void NotifyDirectoryOpened(FileSystemNode directory, List<FileSystemNode> children)
        {
            DirectoryOpened?.Invoke(null, new NodeOpenedEventArgs(directory, children));
        }

        public event EventHandler<NodeChangedEventArgs> StorageEntityCreated;

        public void NotifyStorageEntityCreated(FileSystemNode entity)
        {
            StorageEntityCreated?.Invoke(null, new NodeChangedEventArgs(entity));
        }

        public event EventHandler<NodeChangedEventArgs> StorageEntityDeleted;

        public void NotifyStorageEntityDeleted(FileSystemNode entity)
        {
            StorageEntityDeleted?.Invoke(null, new NodeChangedEventArgs(entity));
        }

        #endregion

        #region ExternalEvents

        public event EventHandler<NewStatsEventArgs> StatsUpdated;

        public void NotifyStatsUpdated(Int32 files, Int32 dirs, Int64 size)
        {
            StatsUpdated?.Invoke(null, new NewStatsEventArgs(files, dirs, size));
        }

        public event EventHandler<EntityAddedEventArgs> EntityAdded;

        public void NotifyEntityAdded(ISharableEntityUi entity)
        {
            EntityAdded?.Invoke(null, new EntityAddedEventArgs(entity));
        }

        public event EventHandler<EntityDeletedEventArgs> EntityDeleted;

        public void NotifyEntityDeleted(CStorageEntityId entityId)
        {
            EntityDeleted?.Invoke(null, new EntityDeletedEventArgs(entityId));
        }

        public event EventHandler<EntityRenamedEventArgs> EntityRenamed;

        public void NotifyEntityRenamed(CStorageEntityId entityId, String newName)
        {
            EntityRenamed?.Invoke(null, new EntityRenamedEventArgs(entityId, newName));
        }

        #endregion

        private readonly HubConnection _connection;

        public FileSystemEventsController(String serviceUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{serviceUrl}/eventshub", options =>
                    {
                        options.AccessTokenProvider = () =>
                            Task.FromResult(CAuthController.CurrentUser.Token);
                    })
                .Build();

            _connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection.StartAsync();
            };

            _connection.On<EntityDeleted>("ReceiveEntityDeleted", ProcessEvent);
            _connection.On<EntityAdded>("ReceiveEntityAdded", ProcessEvent);
            _connection.On<EntityRenamed>("ReceiveEntityRenamed", ProcessEvent);
            _connection.On<StatsUpdated>("ReceiveStatsUpdated", ProcessEvent);
        }


        public void ProcessEvent(EntityDeleted e)
        {
            CStorageEntityId id = new CStorageEntityId(e.DeletedEntityId);
            NotifyEntityDeleted(id);
        }

        public void ProcessEvent(EntityAdded e)
        {
            ISharableEntityUi entity = ContractsConverter.Convert(e.AddedEntity);
            NotifyEntityAdded(entity);
        }

        public void ProcessEvent(EntityRenamed e)
        {
            CStorageEntityId id = new CStorageEntityId(e.EntityId);
            String newName = e.NewName;
            NotifyEntityRenamed(id, newName);
        }

        public void ProcessEvent(StatsUpdated entity)
        {
            Int32 files = entity.UpdatedStats.FilesCount;
            Int32 dirs = entity.UpdatedStats.DirsCount;
            Int64 size = entity.UpdatedStats.StorageSize;
            NotifyStatsUpdated(files, dirs, size);
        }

        public async void Connect()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection to hub failed {ex.Message}");
            }
        }


    }
}
