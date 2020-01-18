using System.Windows.Input;
using BitContainer.Presentation.Controllers;
using BitContainer.Presentation.Controllers.Ui;
using BitContainer.Presentation.ViewModels.Commands;
using Microsoft.Xaml.Behaviors;

namespace BitContainer.Presentation.ViewModels.Base
{
    public class NavigatableViewModelBase : ViewModelBase
    {
        private ICommand _toRegisterPageCommand;

        public ICommand ToRegisterPageCommand => 
            _toRegisterPageCommand ??= new RelayCommand(ToRegisterPage);

        public void ToRegisterPage(object data = null)
        {
            NavigationController.GoToRegisterPage();
        }
        
        public void ToRegisteredPage()
        {
            NavigationController.GoToRegisteredPage();
        }

        private ICommand _toLogInPageCommand;

        public ICommand ToLogInPageCommand => 
            _toLogInPageCommand ??= new RelayCommand(ToLogInPage);

        public void ToLogInPage(object data = null)
        {
            NavigationController.GoToLoginPage();
        }
        
        public void ToMainPage()
        {
            NavigationController.GoToMainPage();
        }


    }
}
