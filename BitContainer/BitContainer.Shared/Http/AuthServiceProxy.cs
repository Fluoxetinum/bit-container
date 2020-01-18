using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Auth;
using BitContainer.Shared.Http.Exceptions;
using BitContainer.Shared.Http.Requests;

namespace BitContainer.Shared.Http
{
    public class AuthServiceProxy
    {
        private static String _serviceUrl = String.Empty;

        private static String _authControllerName = "users";
        private static String _registerRequest => $@"{_serviceUrl}/{_authControllerName}/register/";
        private static String _logInRequest => $@"{_serviceUrl}/{_authControllerName}/logIn/";
        private static String _getUserRequest => $@"{_serviceUrl}/{_authControllerName}/user/";

        public static void SetServiceUrl(String serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        public static async Task<CUserContract> GetUserWithName(String name)
        {
            var requestBuilder = new GetRequestBuilder(_getUserRequest + name);
            return await HttpHelper.Request<CUserContract>(requestBuilder, (response) =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new NoSuchUserException(response.Content.ReadAsStringAsync().Result);
                }
            });
        }

        public static async Task RegisterRequest(CCredentialsContract message)
        {
            var requestBuilder = new PostRequestBuilder<CCredentialsContract>(_registerRequest, message);

            await HttpHelper.Request(requestBuilder, (response) =>
            {
                try 
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new UsernameExistsException(response.Content.ReadAsStringAsync().Result);
                }
            });
        }
        
        public static async Task<CAuthenticatedUserContract> LogInRequest(CCredentialsContract message)
        {
            var requestBuilder = 
                new PostRequestBuilder<CCredentialsContract>
                    (_logInRequest, message);
            
            return await HttpHelper.Request<CAuthenticatedUserContract>(requestBuilder, (response) =>
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new NoSuchUserException(response.Content.ReadAsStringAsync().Result);
                }
            });
        }

    }
}
