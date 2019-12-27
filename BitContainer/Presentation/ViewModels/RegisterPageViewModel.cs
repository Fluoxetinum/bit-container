using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.Proxies.Exceptions;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands;

namespace BitContainer.Presentation.ViewModels
{
    class RegisterPageViewModel : WaitingViewModelBase
    {
        public Boolean MockForPasswordValidation => false;

        private String _password;

        public String Password
        {
            get => _password;
            set
            {
                _password = value;
                HideErrorMessage();
                ValidatePassword();
                OnPropertyChanged();
            }
        }

        private Boolean ValidatePassword()
        {
            if (_password.Length == 0)
            {
                SetFieldIsRequiredError(nameof(MockForPasswordValidation));
                return false;
            }
            ClearErrors(nameof(MockForPasswordValidation));

            return ValidatePasswords();
        }

        public Boolean MockForPasswordConfirmationValidation => false;

        private String _passwordConfirmation;

        public String PasswordConfirmation
        {
            get => _passwordConfirmation;
            set
            {
                _passwordConfirmation = value;
                HideErrorMessage();
                ValidatePasswordConfirmation();
                OnPropertyChanged();
            }
        }

        private Boolean ValidatePasswordConfirmation()
        {
            if (_passwordConfirmation.Length == 0)
            {
                SetFieldIsRequiredError(nameof(MockForPasswordConfirmationValidation));
                return false;
            }
            ClearErrors(nameof(MockForPasswordConfirmationValidation));

            return ValidatePasswords();
        }

        private Boolean ValidatePasswords()
        {
            if (!_password.Equals(_passwordConfirmation))
            {
                SetBlankError(nameof(MockForPasswordValidation));
                SetBlankError(nameof(MockForPasswordConfirmationValidation));
                ShowErrorMessage("Passwords do not match");
                return false;
            }

            ClearErrors(nameof(MockForPasswordValidation));
            ClearErrors(nameof(MockForPasswordConfirmationValidation));
            return true;
        }

        private String _userName;
        
        public String UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                HideErrorMessage();
                ValidateUserName();
                OnPropertyChanged();
            }
        }

        private Boolean ValidateUserName()
        {
            if (_userName.Length == 0)
            {
                SetFieldIsRequiredError(nameof(UserName));
                return false;
            }

            ClearErrors(nameof(UserName));
            return true;
        }

        private String _errorMessage;

        public String ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private void ShowErrorMessage(String message)
        {
            ErrorMessage = message;
        }

        private void HideErrorMessage()
        {
            ErrorMessage = String.Empty;
        }

        public RegisterPageViewModel()
        {
            _userName = String.Empty;
            _password = String.Empty;
            _passwordConfirmation = String.Empty;
        }

        private ICommand _tryRegisterCommand;

        public ICommand TryRegisterCommand =>
            _tryRegisterCommand ?? (_tryRegisterCommand = new RelayCommand(TryRegister));
        
        private async void TryRegister(object data)
        {
            Boolean validPassword = ValidatePassword();
            Boolean validPasswordConfirmation = ValidatePasswordConfirmation();
            Boolean validUserName = ValidateUserName();

            if (!validUserName || !validPassword || !validPasswordConfirmation) return;

            RequestStart();

            try
            {
                await AuthController.Register(_userName, _password);
                ToRegisteredPage();
            }
            catch (UsernameExistsException)
            {
                SetBlankError(nameof(UserName));
                ShowErrorMessage($"User with name {_userName} already exists");
            }
            
            RequestEnd();
        }
    }
}
