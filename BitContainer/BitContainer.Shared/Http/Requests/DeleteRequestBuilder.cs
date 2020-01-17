using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace BitContainer.Shared.Http.Requests
{
    public class DeleteRequestBuilder<TRequest> :  IServiceRequestBuilder
    {
        private String _uri;
        private TRequest _content;

        public DeleteRequestBuilder(String uri, TRequest content)
        {
            _uri = uri;
            _content = content;
        }
        
        public HttpRequestMessage Create()
        {
            return new HttpRequestMessage(HttpMethod.Delete, _uri)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(_content), 
                    Encoding.UTF8, 
                    "application/json")
            };
        }
    }
}
