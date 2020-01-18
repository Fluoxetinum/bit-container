using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Stats;

namespace BitContainer.DataAccess.DataProviders
{
    public class CStatsProvider : IStatsProvider
    {
        private readonly ISqlDbHelper _dbHelper;

        public CStatsProvider(ISqlDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public Int32 AddNewStats(Guid id)
        {
            var newStatsQuery = new AddNewUserStatsQuery(id);
            return _dbHelper.ExecuteQuery(newStatsQuery);
        }

        public CUserStats GetStats(Guid id)
        {
            var query = new GetUserStatsQuery(id);
            return _dbHelper.ExecuteQuery(query);
        }

        public Int32 RemoveStats(Guid id)
        {
            var deleteStatsQuery = new RemoveUserStatsQuery(id);
            return _dbHelper.ExecuteQuery(deleteStatsQuery);
        }
    }
}
