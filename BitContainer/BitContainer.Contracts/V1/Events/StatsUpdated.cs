using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.Contracts.V1.Storage;

namespace BitContainer.Contracts.V1.Events
{
    public class StatsUpdated
    {
        public CStatsContract UpdatedStats { get; set; }

        public StatsUpdated(){}

        public StatsUpdated(CStatsContract stats)
        {
            UpdatedStats = stats;
        }

    }
}
