using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitContainer.LogService.Models;
using BitContainer.Shared.Http;
using Microsoft.AspNetCore.Mvc;

namespace BitContainer.StorageService.Controllers
{
    [Route("logger")]
    [ApiController]
    public class LogsProxyController : ControllerBase
    {
        [HttpPost]
        [Route("log")]
        public async Task<ActionResult> Log(CNlogMessageContract entry)
        {
            await CLogServiceProxy.LogRequest(entry);
            return Ok();
        }
    }
}
