using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BitContainer.Presentation.Controllers.Proxies.Requests
{
    public interface IServiceRequestBuilder
    {
        HttpRequestMessage Create();
    }
}
