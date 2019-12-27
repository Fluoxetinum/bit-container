using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Presentation.Controllers.Proxies.Requests;

namespace BitContainer.Presentation.Controllers.Proxies.Requests
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
