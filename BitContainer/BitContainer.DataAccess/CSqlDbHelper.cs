using System;
using System.Data.SqlClient;
using System.IO;
using System.Data.Sql;
using BitContainer.DataAccess.Queries;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.DataAccess.Scripts;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace BitContainer.DataAccess
{
    public class CSqlDbHelper : ISqlDbHelper
    {
        private readonly String _connectionString;

        public CSqlDbHelper(String sqlServerConnectionString, IT4DbInitScript initScript)
        {
            using (var connection = new SqlConnection(sqlServerConnectionString))
            {
                Server server = new Server(new ServerConnection(connection));
                server.ConnectionContext.ExecuteNonQuery(initScript.TransformText());
            }

            _connectionString = $"{sqlServerConnectionString}Database={initScript.DbName}";
        }
        
        public T ExecuteQuery<T>(ISqlQuery<T> query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                return query.Execute(command);
            }
        }

        public void ExecuteTransaction(Action<SqlCommand> executionAlgorithm)
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
