using System;
using BitContainer.Presentation.ViewModels.Jobs;

namespace BitContainer.Presentation.Controllers.Ui.EventParams
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
