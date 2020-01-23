using System;
using System.Net.Http;
using System.Threading.Tasks;
using BitContainer.Http.Requests;

namespace BitContainer.Http
{
    public interface IHttpHelper
    {
        String Token { set; }
        Task<T> Request<T>(IServiceRequestBuilder builder, Action<HttpResponseMessage> catchAction);
        Task Request(IServiceRequestBuilder builder, Action<HttpResponseMessage> catchAction);
    }
}
