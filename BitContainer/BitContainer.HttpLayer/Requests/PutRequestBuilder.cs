using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace BitContainer.Http.Requests
{
    public class PutRequestBuilder<TRequest> : IServiceRequestBuilder
    {
        private readonly String _uri;
        private readonly TRequest _content;

        public PutRequestBuilder(String uri, TRequest content)
        {
            _uri = uri;
            _content = content;
        }
        
        public HttpRequestMessage Create()
        {
            return new HttpRequestMessage(HttpMethod.Put, _uri)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(_content), 
                    Encoding.UTF8, 
                    "application/json")
            };
        }
    }
}
