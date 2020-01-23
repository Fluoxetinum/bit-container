using System;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.Shared.Models;

namespace BitContainer.DataAccess.DataProviders.Interfaces
{
    public interface IUsersProvider
    {
        Byte[] GetSalt(String userName);
        CUser GetUserWithCredentials(String userName, Byte[] passwordHash);
        CUser GetUserWithName(String userName);

        Int32 AddNewUser(String name, Byte[] passwordHash, Byte[] salt);
        Int32 RemoveUser(CUserId userId);
    } 
}
