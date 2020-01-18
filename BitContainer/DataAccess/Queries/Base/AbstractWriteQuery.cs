using System;
using System.Data;
using System.Data.SqlClient;

namespace BitContainer.DataAccess.Queries.Base
{
    public abstract class AbstractWriteQuery : ISqlQuery<Int32>
    {
        public abstract SqlCommand Prepare(SqlCommand command);

        public Int32 Execute(SqlCommand command)
        {
            SqlCommand preparedCommand = Prepare(command);
            Int32 result = preparedCommand.ExecuteNonQuery();
            preparedCommand.Parameters.Clear();
            command.CommandType = CommandType.Text;
            return result;
        }
    }
}
