using System;
using System.Windows.Input;
using BitContainer.Http.Exceptions;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.Service;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands;

namespace BitContainer.Presentation.ViewModels.Auth
{
    class LogInPageViewModel : WaitingViewModelBase
    {
        private CAuthController _authController;

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


        public LogInPageViewModel(CAuthController authController)
        {
            _userName = String.Empty;
            _password = String.Empty;
            _authController = authController;
        }

        private ICommand _tryLogInCommand;

        public ICommand TryLogInCommand =>
            _tryLogInCommand ??= new RelayCommand(TryLogIn);

        public async void TryLogIn(object data)
        {
            Boolean validPassword = ValidatePassword();
            Boolean validUserName = ValidateUserName();

            if (!validUserName || !validPassword) return;

            RequestStart();

            try
            {
                await _authController.LogIn(_userName, _password);
                ToMainPage();
            }
            catch (NoSuchUserException)
            {
                SetBlankError(nameof(UserName));
                SetBlankError(nameof(MockForPasswordValidation));
                ShowErrorMessage("Invalid username or password");
            }

            RequestEnd();
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
    }
}
