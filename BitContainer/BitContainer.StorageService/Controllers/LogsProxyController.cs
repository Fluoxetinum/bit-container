using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Http.Proxies;
using Microsoft.AspNetCore.Mvc;

namespace BitContainer.Service.Storage.Controllers
{
    [Route("logger")]
    [ApiController]
    public class LogsProxyController : ControllerBase
    {
        private readonly ILogServiceProxy _logSaServiceProxy;

        public LogsProxyController(ILogServiceProxy logServiceProxy)
        {
            _logSaServiceProxy = logServiceProxy;
        }

        [HttpPost]
        [Route("log")]
        public async Task<ActionResult> Log(CNlogMessageContract entry)
        {
            await _logSaServiceProxy.LogRequest(entry);
            return Ok();
        }
    }
}
