using System.Threading.Tasks;
using BitContainer.Contracts.V1.Events;

namespace BitContainer.Service.Storage.Managers.Interfaces
{
    public interface IEventsHubClient
    {
        Task ReceiveEntityAdded(EntityAdded entity);
        Task ReceiveEntityDeleted(EntityDeleted entity);
        Task ReceiveEntityRenamed(EntityRenamed entity);
        Task ReceiveStatsUpdated(StatsUpdated stats);
    }
}
