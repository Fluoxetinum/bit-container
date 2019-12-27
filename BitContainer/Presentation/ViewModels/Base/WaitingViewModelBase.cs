using System;

namespace BitContainer.Presentation.ViewModels.Base
{
    public class WaitingViewModelBase : NavigatableViewModelBase
    {
        private Boolean _requestInProgress;
        public Boolean RequestInProgess
        {
            get => _requestInProgress;
            set
            {
                _requestInProgress = value;
                OnPropertyChanged();
            }
        }

        private Boolean _isCommandsEnabled;
        public Boolean IsCommandsEnabled
        {
            get => _isCommandsEnabled;
            set
            {
                _isCommandsEnabled = value;
                OnPropertyChanged();
            }
        }

        public void RequestStart()
        {
            IsCommandsEnabled = false;
            RequestInProgess = true;
        }

        public void RequestEnd()
        {
            IsCommandsEnabled = true;
            RequestInProgess = false;
        }

        public WaitingViewModelBase()
        {
            _isCommandsEnabled = true;
        }

    }
}
