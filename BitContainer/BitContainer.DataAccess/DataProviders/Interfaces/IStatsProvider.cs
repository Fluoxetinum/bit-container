using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess.Models;

namespace BitContainer.DataAccess.DataProviders.Interfaces
{
    public interface IStatsProvider
    {
        Int32 AddNewStats(Guid id);
        CUserStats GetStats(Guid id);
        Int32 RemoveStats(Guid id);
    }
}
