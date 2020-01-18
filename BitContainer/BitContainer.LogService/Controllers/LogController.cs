using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.DataAccess.DataProviders.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BitContainer.LogService.Controllers
{
    [ApiController]
    [Route("logger")]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> _logServiceLogger;
        private readonly ILogsProvider _universalLogger;

        public LogController(ILogger<LogController> logServiceLogger, ILogsProvider universalLogger)
        {
            _logServiceLogger = logServiceLogger;
            _universalLogger = universalLogger;
        }

        [HttpPost]
        [Route("log")]
        public ActionResult Log(CNlogMessageContract entry)
        {
            _universalLogger.Log(entry.NLogLevel, entry.Message, entry.Exception);
            _logServiceLogger.Log(GetMicrosoftLogLevel(entry.NLogLevel), entry.Message);
            return Ok();
        }

        private static LogLevel GetMicrosoftLogLevel(String nLogLevel)
        {
            switch (nLogLevel)
            {
                case "Fatal":
                    return LogLevel.Critical;
                case "Error":
                    return LogLevel.Error;
                case "Warn":
                    return LogLevel.Warning;
                case "Info":
                    return LogLevel.Information;
                case "Debug":
                    return LogLevel.Debug;
                case "Trace":
                    return LogLevel.Trace;
                default:
                    throw new InvalidOperationException("Unexpected NlogLevel type.");
            }
        }
    }
}
