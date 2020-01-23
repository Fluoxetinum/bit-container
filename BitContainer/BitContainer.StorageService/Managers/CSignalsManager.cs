using System.Collections.Generic;
using System.Linq;
using BitContainer.Contracts.V1.Events;
using BitContainer.Contracts.V1.Storage;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.Service.Storage.Helpers;
using BitContainer.Service.Storage.Managers.Interfaces;
using BitContainer.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace BitContainer.Service.Storage.Managers
{
    public class CSignalsManager : ISignalsManager
    {
        private readonly IHubContext<CEventsHub, IEventsHubClient> _hubContext;
        private readonly IStorageProvider _storageProvider;

        public CSignalsManager(IHubContext<CEventsHub, IEventsHubClient> hubContext, IStorageProvider storageProvider)
        {
            _hubContext = hubContext;
            _storageProvider = storageProvider;
        }

        private List<string> GetStrUserIds(CStorageEntityId id, CUserId actorId)
        {
            IStorageEntity entity = _storageProvider.Entities.GetStorageEntity(id);
            List<CUserId> ids = _storageProvider.Shares.GetShares(id)
                .Select(s => s.UserId)
                .ToList();
            ids.Add(entity.OwnerId);
            ids.Remove(actorId);
            return ids.Select(i => i.ToString()).ToList();
        }

        public void SignalStatsUpdate(CUserId id)
        {
            CStats stats = _storageProvider.Stats.GetStats(id);
            CStatsContract contract = ContractsConverter.Convert(stats);
            StatsUpdated e = new StatsUpdated(contract);
            _hubContext.Clients.User(id.ToString()).ReceiveStatsUpdated(e);
        }

        public void SignalEntityCreation(CStorageEntityId id, CUserId actorId)
        {
            var sharableEntities = new Dictionary<CUserId, CSharableEntity>();
            var entity = _storageProvider.Entities.GetStorageEntity(id);
            var shares = _storageProvider.Shares.GetShares(id);

            CAccessWrapper ownerWrapper = new CAccessWrapper(entity, EAccessType.Write);
            sharableEntities.Add(entity.OwnerId, new CSharableEntity(ownerWrapper, shares));
            foreach (var s in shares)
            {
                CAccessWrapper accessWrapper = new CAccessWrapper(entity, s.AccessType);
                sharableEntities.Add(s.UserId, new CSharableEntity(accessWrapper, shares));
            }

            sharableEntities.Remove(actorId);

            foreach (var pair in sharableEntities)
            {
                EntityAdded e = new EntityAdded(ContractsConverter.Convert(pair.Value));
                _hubContext.Clients.User(pair.Key.ToString()).ReceiveEntityAdded(e);
            }
        }

        public void SignalEntityDeletion(CStorageEntityId id, CUserId actorId)
        {
            EntityDeleted e = new EntityDeleted(id.ToGuid());
            List<string> strIds = GetStrUserIds(id, actorId);
            _hubContext.Clients.Users(strIds).ReceiveEntityDeleted(e);
        }

        public void SignalEntityRenaming(CStorageEntityId id, string newName, CUserId actorId)
        {
            EntityRenamed e = new EntityRenamed(id.ToGuid(), newName);
            List<string> strIds = GetStrUserIds(id, actorId);
            _hubContext.Clients.Users(strIds).ReceiveEntityRenamed(e);
        }
    }
}
