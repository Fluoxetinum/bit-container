using System;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.DataProviders.Interfaces
{
    public interface IUsersProvider
    {
        Byte[] GetSalt(String userName);
        CUser GetUserWithCredentials(String name, Byte[] passwordHash);
        CUser GetUserWithName(String name);

        Int32 AddNewUser(String name, Byte[] passwordHash, Byte[] salt);
        Int32 RemoveUser(Guid id);
    } 
}
