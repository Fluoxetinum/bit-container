using System;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Http.Requests;

namespace BitContainer.Http.Proxies
{
    public class CLogServiceProxy : ILogServiceProxy
    {
        private readonly IHttpHelper _httpHelper;

        private static readonly String _authControllerName = "logger";
        private readonly String _logRequest;

        public CLogServiceProxy(IHttpHelper httpHelper, String serviceUrl)
        {
            _httpHelper = httpHelper;
            _logRequest = $@"{serviceUrl}/{_authControllerName}/log/";
        }
        
        public async Task LogRequest(CNlogMessageContract message)
        {
            var requestBuilder = new PostRequestBuilder<CNlogMessageContract>(_logRequest, message);
            await _httpHelper.Request(requestBuilder, CHttpHelper.EmptyCatchAction);
        }
    }
}
