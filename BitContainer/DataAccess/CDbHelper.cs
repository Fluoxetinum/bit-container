using System;
using System.Data.SqlClient;
using System.IO;
using System.Data.Sql;
using BitContainer.DataAccess.Queries;
using BitContainer.DataAccess.Queries.Base;

namespace BitContainer.DataAccess
{
    public class CDbHelper
    {
        private static String _connectionString;

        public static void Init(String sqlServerConnectionString)
        {
            _connectionString = sqlServerConnectionString;
        }
        
        public static T ExecuteQuery<T>(IQuery<T> query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                return query.Execute(command);
            }
        }

        public static void ExecuteTransaction(Action<SqlCommand> executionAlgorithm)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction = connection.BeginTransaction();

                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    executionAlgorithm(command);

                    transaction.Commit();
                }
                catch (Exception queryEx)
                {
                    //TODO: Implement exception handling

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        throw;
                    }

                    throw;
                }
            }
        }
    }
}
