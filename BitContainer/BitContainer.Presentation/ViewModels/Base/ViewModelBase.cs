using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BitContainer.Presentation.Annotations;

namespace BitContainer.Presentation.ViewModels.Base
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly Dictionary<String, List<String>> _errors = new Dictionary<String, List<String>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public Boolean HasErrors => _errors.Count > 0;

        protected void SetErrors(String propertyName, List<String> propertyErrors)
        {
            _errors.Remove(propertyName);
            _errors.Add(propertyName, propertyErrors);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void SetFieldIsRequiredError(String propertyName)
        {
            SetErrors(propertyName, new List<string>()
            {
                "This field is required"
            });
        }

        protected void SetBlankError(String propertyName)
        {
            SetErrors(propertyName, new List<string>()
            {
                ""
            });
        }

        public IEnumerable GetErrors(String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return (_errors.Values);
            }

            if (_errors.ContainsKey(propertyName))
            {
                return (_errors[propertyName]);
            }

            return null;
        }

        protected void ClearErrors(String propertyName)
        {
            _errors.Remove(propertyName);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
