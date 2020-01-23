using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Jobs;

namespace BitContainer.Presentation.Controllers.EventParams
{
    public class AbstractJobEventArgs : EventArgs
    {
        public readonly AbstractJob Job;

        public AbstractJobEventArgs(AbstractJob job)
        {
            Job = job;
        }
    }
}
