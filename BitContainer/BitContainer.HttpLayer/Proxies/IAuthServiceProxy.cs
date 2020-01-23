using System;
using System.Threading.Tasks;
using BitContainer.Contracts.V1.Auth;

namespace BitContainer.Http.Proxies
{
    public interface IAuthServiceProxy
    {
        Task<CUserContract> GetUserWithName(String name);
        Task RegisterRequest(CCredentialsContract message);
        Task<CAuthenticatedUserContract> LogInRequest(CCredentialsContract message);
        void LogOut();
    }
}
