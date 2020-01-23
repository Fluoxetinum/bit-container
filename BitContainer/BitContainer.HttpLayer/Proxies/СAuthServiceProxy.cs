using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BitContainer.Contracts.V1.Auth;
using BitContainer.Http.Exceptions;
using BitContainer.Http.Requests;

namespace BitContainer.Http.Proxies
{
    public class СAuthServiceProxy : IAuthServiceProxy
    {
        private readonly IHttpHelper _httpHelper;

        private void UpdateToken(string token) => _httpHelper.Token = token;

        private static readonly String _authControllerName = "users";
        private static String _registerRequest;
        private static String _logInRequest;
        private static String _getUserRequest;

        public СAuthServiceProxy(IHttpHelper httpHelper, String serviceUrl)
        {
            _httpHelper = httpHelper;
            _registerRequest = $@"{serviceUrl}/{_authControllerName}/register/";
            _logInRequest = $@"{serviceUrl}/{_authControllerName}/logIn/";
            _getUserRequest = $@"{serviceUrl}/{_authControllerName}/user/";
        }
        
        public async Task<CUserContract> GetUserWithName(String name)
        {
            var requestBuilder = new GetRequestBuilder(_getUserRequest + name);
            return await _httpHelper.Request<CUserContract>(requestBuilder, ServiceErrorsCatcher);
        }

        public async Task RegisterRequest(CCredentialsContract message)
        {
            var requestBuilder = new PostRequestBuilder<CCredentialsContract>(_registerRequest, message);
            await _httpHelper.Request(requestBuilder, ServiceErrorsCatcher);
        }
        
        public async Task<CAuthenticatedUserContract> LogInRequest(CCredentialsContract message)
        {
            var requestBuilder = new PostRequestBuilder<CCredentialsContract>(_logInRequest, message);
            CAuthenticatedUserContract contract = await _httpHelper.Request<CAuthenticatedUserContract>(requestBuilder, ServiceErrorsCatcher);
            UpdateToken(contract.Token);
            return contract;
        }

        public void LogOut()
        {
            UpdateToken(null);
        }

        private void ServiceErrorsCatcher(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new NoSuchUserException(response.Content.ReadAsStringAsync().Result);
            }
            catch (HttpRequestException) when (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new UsernameExistsException(response.Content.ReadAsStringAsync().Result);
            }
        }

    }
}
