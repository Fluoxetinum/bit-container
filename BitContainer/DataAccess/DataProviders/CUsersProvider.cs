using System;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Auth;
using BitContainer.DataAccess.Queries.Stats;

namespace BitContainer.DataAccess.DataProviders
{
    public class CUsersProvider : IUsersProvider
    {
        private readonly ISqlDbHelper _dbHelper;

        public CUsersProvider(ISqlDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public Int32 AddNewUser(String name, Byte[] passwordHash, Byte[] salt)
        {
            var query = new AddNewUserQuery(name, passwordHash, salt);
            return _dbHelper.ExecuteQuery(query);
        }
        
        public Byte[] GetSalt(String userName)
        {
            var query = new GetSaltQuery(userName);
            return _dbHelper.ExecuteQuery(query);
        }

        public CUser GetUserWithCredentials(String name, Byte[] passwordHash)
        {
            var query = new GetUserWithCredentialsQuery(name, passwordHash);
            return _dbHelper.ExecuteQuery(query);
        }

        public CUser GetUserWithName(String name)
        {
            var query = new GetUserWithNameQuery(name);
            return _dbHelper.ExecuteQuery(query);
        }

        public Int32 RemoveUser(Guid id)
        {
            Int32 result = -1;
            _dbHelper.ExecuteTransaction(executionAlgorithm:(command) =>
            {
                var query = new RemoveUserQuery(id);
                result = query.Execute(command);
            });
            return result;
        }
    }
}
