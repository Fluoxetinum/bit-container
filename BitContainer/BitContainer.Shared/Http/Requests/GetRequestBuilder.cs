using System;
using System.Net.Http;

namespace BitContainer.Shared.Http.Requests
{
    public class GetRequestBuilder : IServiceRequestBuilder
    {
        private String _uri;

        public GetRequestBuilder(String uri)
        {
            _uri = uri;
        }
        
        public HttpRequestMessage Create()
        {
            return new HttpRequestMessage(HttpMethod.Get, _uri);
        }
    }
}
