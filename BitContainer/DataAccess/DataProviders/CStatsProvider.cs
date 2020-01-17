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
        public Int32 AddNewStats(Guid id)
        {
            var newStatsQuery = new AddNewUserStatsQuery(id);
            return CDbHelper.ExecuteQuery(newStatsQuery);
        }

        public CUserStats GetStats(Guid id)
        {
            var query = new GetUserStatsQuery(id);
            return CDbHelper.ExecuteQuery(query);
        }
    }
}
