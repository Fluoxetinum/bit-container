using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Model;
using BitContainer.Presentation.ViewModels.Base;
using BitContainer.Presentation.ViewModels.Commands;

namespace BitContainer.Presentation.ViewModels
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
