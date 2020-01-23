using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.DataAccess.Scripts;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace BitContainer.DataAccess
{
    public class CSqlDbHelper : ISqlDbHelper
    {
        private readonly String _connectionString;
        private readonly ILogger<CSqlDbHelper> _logger;

        public CSqlDbHelper(String sqlServerConnectionString, IT4DbInitScript initScript, ILogger<CSqlDbHelper> logger)
        {
            using (var connection = new SqlConnection(sqlServerConnectionString))
            {
                Server server = new Server(new ServerConnection(connection));
                server.ConnectionContext.ExecuteNonQuery(initScript.TransformText());
            }

            _connectionString = $"{sqlServerConnectionString}Database={initScript.DbName}";
            _logger = logger;
        }
        
        public T ExecuteQuery<T>(ISqlQuery<T> query)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            return query.Execute(command);
        }

        public void ExecuteTransaction(Action<SqlCommand> executionAlgorithm)
        {
            using var connection = new SqlConnection(_connectionString);
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
                _logger.LogError(queryEx, $"Transaction on '{transaction.Connection.ConnectionString}' failed," +
                                          $" last command - {command.CommandText}");

                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, $"Rollback on '{transaction.Connection.ConnectionString}' failed," +
                                                 $" last command - {command.CommandText}");
                    throw;
                }

            }
        }

        public async Task ExecuteTransactionAsync(Func<SqlCommand, Task> executionAlgorithm)
        {
            await using var connection = new SqlConnection(_connectionString);

            connection.Open();
            SqlCommand command = connection.CreateCommand();
            SqlTransaction transaction = connection.BeginTransaction();

            command.Connection = connection;
            command.Transaction = transaction;
            try
            {
                await executionAlgorithm(command);
                transaction.Commit();
            }
            catch (Exception queryEx)
            {
                _logger.LogError(queryEx, $"Transaction on '{transaction.Connection.ConnectionString}' failed," +
                                          $" last command - {command.CommandText}");
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, $"Rollback on '{transaction.Connection.ConnectionString}' failed," +
                                                 $" last command - {command.CommandText}");
                    throw;
                }

            }
        }
    }
}
