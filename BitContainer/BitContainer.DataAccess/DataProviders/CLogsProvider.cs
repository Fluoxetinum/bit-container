using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Queries;

namespace BitContainer.DataAccess.DataProviders
{
    public class CLogsProvider : ILogsProvider
    {
        private readonly ISqlDbHelper _dbHelper;

        public CLogsProvider(ISqlDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public void Log(string level, string message, string exception)
        {
            var query = new CLogQuery(level, message, exception);
            _dbHelper.ExecuteQuery(query);
        }
    }
}
