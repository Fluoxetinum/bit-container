using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.Views;

namespace BitContainer.Presentation.Controllers.Ui
{
    public static class NavigationController
    {
        private static readonly NavigationService _navigationService;
        
        static NavigationController()
        {
            NavigationWindow nw = (NavigationWindow) Application.Current.MainWindow;
            _navigationService = nw.NavigationService;
        }

        public static void GoToMainPage()
        {
            GoTo(EPage.Explorer);
        }

        public static void GoToLoginPage()
        {
            GoTo(EPage.LogIn);
        }

        public static void GoToRegisterPage()
        {
            GoTo(EPage.Register);
        }

        public static void GoToRegisteredPage()
        {
            GoTo(EPage.RegisterSuccess);
        }

        public static void GoTo(EPage page)
        {
            Page oldPage = (Page)_navigationService.Content;

            if (oldPage != null)
            {
                if (oldPage.DataContext is IDisposable disposable)
                    disposable.Dispose();
            }
            
            Page newPage;

            switch (page)
            {
                case EPage.LogIn:
                    newPage = new LogInPage();
                    newPage.DataContext = new LogInPageViewModel();
                    break;
                case EPage.Register:
                    newPage = new RegisterPage();
                    newPage.DataContext = new RegisterPageViewModel();
                    break;
                case EPage.RegisterSuccess:
                    newPage = new RegisteredPage();
                    newPage.DataContext = new RegisteredPageViewModel();
                    break;
                case EPage.Explorer:
                    newPage = new MainPage();
                    newPage.DataContext = new MainPageViewModel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }

            _navigationService.Navigate(newPage);
        }


    }
}
