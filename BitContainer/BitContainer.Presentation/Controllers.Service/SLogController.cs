using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace BitContainer.Presentation.Controllers
{
    public static class SLogController
    {
        public static Logger Logger { get; }

        static SLogController()
        {
            LogFactory logFactory = LogManager.LoadConfiguration("nlog.config");
            Logger = logFactory.GetCurrentClassLogger();
        }
    }
}
