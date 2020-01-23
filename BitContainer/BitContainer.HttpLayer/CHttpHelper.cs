using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BitContainer.Http.Requests;
using Newtonsoft.Json;

namespace BitContainer.Http
{
    public class CHttpHelper : IHttpHelper
    {
        public static readonly Action<HttpResponseMessage> EmptyCatchAction = response => { };

        private readonly HttpClient _httpClient = new HttpClient();

        public String Token
        {
            set => _httpClient.DefaultRequestHeaders.Authorization = 
                AuthenticationHeaderValue.Parse($"Bearer {value}");
        }

        public async Task<T> Request<T>(IServiceRequestBuilder builder, Action<HttpResponseMessage> catchAction)
        {
            HttpResponseMessage response = await ProcessCommunicationCycle(builder, catchAction);

            if (response.StatusCode != HttpStatusCode.OK)
                return default(T);

            String jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }

        public async Task Request(IServiceRequestBuilder builder, Action<HttpResponseMessage> catchAction)
        {
            await ProcessCommunicationCycle(builder, catchAction);
        }

        private async Task<HttpResponseMessage> ProcessCommunicationCycle(IServiceRequestBuilder builder, 
            Action<HttpResponseMessage> catchAction)
        {
            HttpRequestMessage request = builder.Create();
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            
            catchAction(response);

            return response;
        }
    }
}
