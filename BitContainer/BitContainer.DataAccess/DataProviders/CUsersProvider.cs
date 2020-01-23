using System;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Queries.Auth;
using BitContainer.Shared.Models;

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

        public CUser GetUserWithCredentials(String userName, Byte[] passwordHash)
        {
            var query = new GetUserWithCredentialsQuery(userName, passwordHash);
            return _dbHelper.ExecuteQuery(query);
        }

        public CUser GetUserWithName(String userName)
        {
            var query = new GetUserWithNameQuery(userName);
            return _dbHelper.ExecuteQuery(query);
        }

        public Int32 RemoveUser(CUserId userId)
        {
            var query = new RemoveUserQuery(userId);
            return _dbHelper.ExecuteQuery(query);
        }
    }
}
