using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BitContainer.Shared.Http.Requests;
using Newtonsoft.Json;

namespace BitContainer.Shared.Http
{
    public static class HttpHelper
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static  readonly Action<HttpResponseMessage> EmptyCatchAction = response => { };

        static HttpHelper()
        {
            JsonConvert.DefaultSettings = () =>
            {
                JsonSerializerSettings settings = 
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                return settings;
            };
        }

        public static void UpdateToken(String token)
        {
            HttpClient.DefaultRequestHeaders.Authorization = 
                AuthenticationHeaderValue.Parse($"Bearer {token}");
        }

        public static async Task<T> Request<T>(IServiceRequestBuilder builder, 
            Action<HttpResponseMessage> catchAction)
        {
            HttpResponseMessage response = await ProcessCommunicationCycle(builder, catchAction);

            if (response.StatusCode != HttpStatusCode.OK)
                return default(T);

            String jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }

        public static async Task Request(IServiceRequestBuilder builder, Action<HttpResponseMessage> catchAction)
        {
            await ProcessCommunicationCycle(builder, catchAction);
        }

        private static async Task<HttpResponseMessage> ProcessCommunicationCycle(IServiceRequestBuilder builder, 
            Action<HttpResponseMessage> catchAction)
        {
            HttpRequestMessage request = builder.Create();
            HttpResponseMessage response = await HttpClient.SendAsync(request);
            
            catchAction(response);

            return response;
        }

       


    }
}
