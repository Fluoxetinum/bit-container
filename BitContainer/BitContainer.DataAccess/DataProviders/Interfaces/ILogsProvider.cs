using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.DataProviders.Interfaces
{
    public interface ILogsProvider
    {
        void Log(String level, String message, String exception);
    }
}
