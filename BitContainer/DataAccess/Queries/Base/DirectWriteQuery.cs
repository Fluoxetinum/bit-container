using System;
using System.Data.Sql;
using System.Data.SqlClient;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries
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
