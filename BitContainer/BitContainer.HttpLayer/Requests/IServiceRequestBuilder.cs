using System.Net.Http;

namespace BitContainer.Http.Requests
{
    public interface IServiceRequestBuilder
    {
        HttpRequestMessage Create();
    }
}
