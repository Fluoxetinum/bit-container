using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess
{
    public interface ISqlDbHelper
    {
        T ExecuteQuery<T>(ISqlQuery<T> query);
        void ExecuteTransaction(Action<SqlCommand> executionAlgorithm);
        Task ExecuteTransactionAsync(Func<SqlCommand, Task> executionAlgorithm);
    }
}
