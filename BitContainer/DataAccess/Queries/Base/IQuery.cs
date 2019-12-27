using System;
using System.Data.Sql;
using System.Data.SqlClient;

namespace BitContainer.DataAccess.Queries.Base
{
    public interface IQuery<out TResult>
    {
        SqlCommand Prepare(SqlCommand command);
        TResult Execute(SqlCommand command);
    }

    public interface IParametrisedQuery<in TInput, out TResult>
    {
        SqlCommand Prepare(SqlCommand command, TInput parameter);
        TResult Execute(SqlCommand command);
    }
}
