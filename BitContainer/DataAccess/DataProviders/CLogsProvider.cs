using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Queries;

namespace BitContainer.DataAccess.DataProviders
{
    public class CLogsProvider : ILogsProvider
    {
        public void Log(string level, string message, string exception)
        {
            var query = new CLogQuery(level, message, exception);
            CDbHelper.ExecuteQuery(query);
        }
    }
}
