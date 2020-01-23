using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Presentation.Controllers.Ui.EventParams;
using BitContainer.Presentation.ViewModels.Base;
using MaterialDesignThemes.Wpf;

namespace BitContainer.Presentation.ViewModels.Jobs
{
    public abstract class AbstractJob : ViewModelBase
    {
        public static event EventHandler<AbstractJobEventArgs> JobCreated;

        public static void NotifyJobCreated(AbstractJob job)
        {
            JobCreated?.Invoke(null, new AbstractJobEventArgs(job));
        }
        
        private PackIconKind _icon;
        public PackIconKind Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }

        private String _name;
        public String Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private Double _progress;
        public Double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
    }
}
