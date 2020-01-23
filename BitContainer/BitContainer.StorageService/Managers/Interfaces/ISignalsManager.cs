using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitContainer.Shared.Models;

namespace BitContainer.Service.Storage.Managers.Interfaces
{
    public interface ISignalsManager
    {
        public void SignalStatsUpdate(CUserId id);
        public void SignalEntityCreation(CStorageEntityId id, CUserId actorId);
        public void SignalEntityDeletion(CStorageEntityId id, CUserId actorId);
        public void SignalEntityRenaming(CStorageEntityId id, string newName, CUserId actorId);
    }
}
