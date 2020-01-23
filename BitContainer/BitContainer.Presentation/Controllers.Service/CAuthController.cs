using System;
using System.Threading.Tasks;
using BitContainer.Contracts.V1.Auth;
using BitContainer.Http.Proxies;
using BitContainer.Presentation.Helpers;
using BitContainer.Presentation.Models;
using BitContainer.Shared.Auth;

namespace BitContainer.Presentation.Controllers.Service
{
    public class CAuthController
    {
        public static CUserUi CurrentUser { get; set; }

        private readonly IAuthServiceProxy _authServiceProxy;

        public CAuthController(IAuthServiceProxy authServiceProxy)
        {
            _authServiceProxy = authServiceProxy;
        }

        public async Task Register(String userName, String password)
        {
            Byte[] passwordHash = CryptoHelper.GenerateHashWithStaticSalt(password);
            CCredentialsContract message = CCredentialsContract.Create(userName, passwordHash);

            await _authServiceProxy.RegisterRequest(message);
        }

        public async Task LogIn(String userName, String password)
        {
            Byte[] passwordHash = CryptoHelper.GenerateHashWithStaticSalt(password);
            CCredentialsContract message = CCredentialsContract.Create(userName, passwordHash);

            CAuthenticatedUserContract user = await _authServiceProxy.LogInRequest(message);
            UpdateAuthenticatedUser(user);
        }

        private static void UpdateAuthenticatedUser(CAuthenticatedUserContract authInfo)
        {
            CurrentUser = ContractsConverter.Convert(authInfo);
        }

        public static void LogOut()
        {
            CurrentUser = null;
        }

    }
}
