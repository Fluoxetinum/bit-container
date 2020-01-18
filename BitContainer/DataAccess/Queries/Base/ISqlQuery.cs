using System;
using System.Data.Sql;
using System.Data.SqlClient;

namespace BitContainer.DataAccess.Queries.Base
{
    public interface ISqlQuery<out TResult>
    {
        SqlCommand Prepare(SqlCommand command);
        TResult Execute(SqlCommand command);
    }
}
