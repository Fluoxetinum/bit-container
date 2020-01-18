using System;
using System.Windows.Input;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands;

namespace BitContainer.Presentation.ViewModels.Controls
{
    public class MenuControlViewModel : NavigatableViewModelBase
    {
        public CUserUiModel CurrentUserUiModel => AuthController.AuthenticatedUserUiModel;

        private ICommand _logOutCommand;

        public ICommand LogOutCommand =>
            _logOutCommand ??= new RelayCommand(LogOut);

        public void LogOut(Object data)
        {
            AuthController.LogOut();
            NavigationController.GoToLoginPage();
        }
    }
}
