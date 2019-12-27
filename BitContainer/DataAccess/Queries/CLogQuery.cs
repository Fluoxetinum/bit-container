using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess.Queries
{
    public class CLogQuery : AbstractWriteQuery
    {
        public String Level { get; set; }
        public String Message { get; set; }
        public String Exception { get; set; }

        private static readonly String QueryString = $"INSERT INTO Logs (LogLevel, Message, Exception) " +
                                      $"VALUES (@{nameof(Level)}, @{nameof(Message)}, @{nameof(Exception)});";

        public CLogQuery(String level, String message, String exception)
        {
            Level = level;
            Message = message;
            Exception = exception;
        }

        public override SqlCommand Prepare(SqlCommand command)
        {
            command.CommandText = QueryString;
            command.Parameters.AddWithValue(nameof(Message), Message);
            command.Parameters.AddWithValue(nameof(Level), Level);
            command.Parameters.AddWithValue(nameof(Exception),  Exception.GetDbNullIfEmpty());
            return command;
        }
    }
}
