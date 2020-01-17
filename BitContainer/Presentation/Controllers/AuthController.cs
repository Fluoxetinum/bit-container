using System;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Auth;
using BitContainer.Presentation.Controllers.Proxies;
using BitContainer.Presentation.Models;
using BitContainer.Shared;
using BitContainer.Shared.Auth;
using BitContainer.Shared.Http;
using Newtonsoft.Json;

namespace BitContainer.Presentation.Controllers
{
    public static class AuthController
    {
        static AuthController()
        {
            AuthServiceProxy.SetServiceUrl(ConfigurationManager.AppSettings["ServiceUrl"]);
        }

        public static CUserUiModel AuthenticatedUserUiModel { get; set; }
        
        public static async Task Register(String userName, String password)
        {
            Byte[] passwordHash = CryptoHelper.GenerateHashWithStaticSalt(password);
            CCredentialsContract message = CCredentialsContract.Create(userName, passwordHash);

            await AuthServiceProxy.RegisterRequest(message);
        }

        public static async Task LogIn(String userName, String password)
        {
            Byte[] passwordHash = CryptoHelper.GenerateHashWithStaticSalt(password);
            CCredentialsContract message = CCredentialsContract.Create(userName, passwordHash);

            CAuthenticatedUserContract user = await AuthServiceProxy.LogInRequest(message);
            UpdateAuthenticatedUser(user);
        }

        private static void UpdateAuthenticatedUser(CAuthenticatedUserContract authInfo)
        {
            AuthenticatedUserUiModel = CUserUiModel.Create(authInfo);
            HttpHelper.UpdateToken(authInfo.Token);
        }

        public static void LogOut()
        {
            AuthenticatedUserUiModel = null;
            HttpHelper.UpdateToken(null);
        }

    }
}
