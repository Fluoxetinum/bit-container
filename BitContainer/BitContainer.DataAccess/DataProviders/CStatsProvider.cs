using System.Data.SqlClient;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Queries.Stats;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.DataProviders
{
    public class CStatsProvider : IStatsProvider
    {
        private readonly ISqlDbHelper _dbHelper;

        public CStatsProvider(ISqlDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        
        public void AddNewStats(SqlCommand command, CUserId userId)
        {
            var newStatsQuery = new AddNewStatsQuery(userId);
            newStatsQuery.Execute(command);
        }

        public void RemoveStats(SqlCommand command, CUserId userId)
        {
            var deleteStatsQuery = new DeleteStatsQuery(userId);
            deleteStatsQuery.Execute(command);
        }

        public void UpdateStats(SqlCommand command, CStats stats)
        {
            var statsQuery = new UpdateStatsQuery(stats);
            statsQuery.Execute(command);
        }

        public CStats GetStats(SqlCommand command, CUserId userId)
        {
            var statsQuery = new GetStatsQuery(userId);
            return statsQuery.Execute(command);
        }

        public CStats GetStats(CUserId userId)
        {
            var query = new GetStatsQuery(userId);
            return _dbHelper.ExecuteQuery(query);
        }
    }
}
