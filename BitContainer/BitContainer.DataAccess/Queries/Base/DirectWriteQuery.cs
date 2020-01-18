using System;
using System.Data.SqlClient;

namespace BitContainer.DataAccess.Queries.Base
{
    public class DirectWriteQuery : AbstractWriteQuery
    {
        private readonly String _queryString;

        public DirectWriteQuery(String queryString)
        {
            _queryString = queryString;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = _queryString;
            return command;
        }
    }
}
