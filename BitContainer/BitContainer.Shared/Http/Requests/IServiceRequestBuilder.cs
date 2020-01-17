using System.Net.Http;

namespace BitContainer.Shared.Http.Requests
{
    public interface IServiceRequestBuilder
    {
        HttpRequestMessage Create();
    }
}
