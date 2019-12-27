using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace BitContainer.Presentation.Controllers
{
    public static class LogController
    {
        private static Logger _logger;
        public static Logger Logger => _logger;
        
        static LogController()
        {
            LogFactory logFactory = LogManager.LoadConfiguration("nlog.config");
            _logger = logFactory.GetCurrentClassLogger();
        }
    }
}
