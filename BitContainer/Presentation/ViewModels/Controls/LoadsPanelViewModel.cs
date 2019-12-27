﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.EventParams;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Jobs;

namespace BitContainer.Presentation.ViewModels
{
    public class LoadsPanelViewModel : ViewModelBase, IDisposable
    {
        private ObservableCollection<AbstractJob> _jobs;

        public ObservableCollection<AbstractJob> Jobs
        {
            get => _jobs;
            set
            {
                _jobs = value;
                OnPropertyChanged();
            }
        }

        public LoadsPanelViewModel()
        {
            _jobs = new ObservableCollection<AbstractJob>();
            AbstractJob.JobCreated += EventsControllerOnJobCreated;
        }

        public void EventsControllerOnJobCreated(object sender, AbstractJobEventArgs e)
        {
            _jobs.Add(e.Job);
        }

        public void Dispose()
        {
            AbstractJob.JobCreated -= EventsControllerOnJobCreated;
        }
    }
}
