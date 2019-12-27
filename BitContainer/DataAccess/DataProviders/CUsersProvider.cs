using System;
using BitContainer.DataAccess.DataProviders.Interfaces;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries;
using BitContainer.DataAccess.Queries.Auth;
using BitContainer.DataAccess.Queries.Stats;

namespace BitContainer.DataAccess.DataProviders
{
    public class CUsersProvider : IUsersProvider
    {
        public Int32 AddNewUser(String name, Byte[] passwordHash, Byte[] salt)
        {
            Int32 result = -1;
            CDbHelper.ExecuteTransaction(executionAlgorithm:(command) =>
            {
                var query = new AddNewUserQuery(name, passwordHash, salt);
                query.Execute(command);

                var userQuery = new GetUserWithNameQuery(name);
                CUser user = userQuery.Execute(command);

                var newStatsQuery = new AddNewUserStatsQuery(user.Id);
                result = newStatsQuery.Execute(command);
            });
            return result;
        }

        public Byte[] GetSalt(String userName)
        {
            var query = new GetSaltQuery(userName);
            return CDbHelper.ExecuteQuery(query);
        }

        public CUser GetUserWithCredentials(String name, Byte[] passwordHash)
        {
            var query = new GetUserWithCredentialsQuery(name, passwordHash);
            return CDbHelper.ExecuteQuery(query);
        }

        public CUser GetUserWithName(String name)
        {
            var query = new GetUserWithNameQuery(name);
            return CDbHelper.ExecuteQuery(query);
        }

        public Int32 RemoveUser(Guid id)
        {
            Int32 result = -1;
            CDbHelper.ExecuteTransaction(executionAlgorithm:(command) =>
            {
                var deleteStatsQuery = new RemoveUserStatsQuery(id);
                result = deleteStatsQuery.Execute(command);

                var query = new RemoveUserQuery(id);
                result = query.Execute(command);
            });
            return result;
        }

        public CUserStats GetStats(Guid id)
        {
            var query = new GetUserStatsQuery(id);
            return CDbHelper.ExecuteQuery(query);
        }

    }
}
