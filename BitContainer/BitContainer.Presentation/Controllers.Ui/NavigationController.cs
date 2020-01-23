using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using BitContainer.Http;
using BitContainer.Http.Proxies;
using BitContainer.Presentation.Controllers.Proxies;
using BitContainer.Presentation.Controllers.Service;
using BitContainer.Presentation.ViewModels;
using BitContainer.Presentation.ViewModels.Auth;
using BitContainer.Presentation.Views;
using LogInPage = BitContainer.Presentation.Views.Auth.LogInPage;
using RegisteredPage = BitContainer.Presentation.Views.Auth.RegisteredPage;
using RegisterPage = BitContainer.Presentation.Views.Auth.RegisterPage;

namespace BitContainer.Presentation.Controllers.Ui
{
    public static class NavigationController
    {
        private static CAuthController _authController;
        private static FileSystemController _fileSystemController;

        private static readonly NavigationService NavigationService;
        
        static NavigationController()
        {
            NavigationWindow nw = (NavigationWindow) Application.Current.MainWindow;
            NavigationService = nw.NavigationService;
        }
        
        public static void NewSession()
        {
            _authController = DependecyController.GetAuthController();
        }

        public static void InitFs()
        {
            _fileSystemController = DependecyController.GetFileSystemController();
        }

        public static void GoToMainPage()
        {
            InitFs();
            GoTo(EPage.Explorer);
        }

        public static void GoToLoginPage()
        {
            NewSession();
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
            Page oldPage = (Page)NavigationService.Content;

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
                    newPage.DataContext = new LogInPageViewModel(_authController);
                    break;
                case EPage.Register:
                    newPage = new RegisterPage();
                    newPage.DataContext = new RegisterPageViewModel(_authController);
                    break;
                case EPage.RegisterSuccess:
                    newPage = new RegisteredPage();
                    newPage.DataContext = new RegisteredPageViewModel();
                    break;
                case EPage.Explorer:
                    newPage = new MainPage();
                    newPage.DataContext = new MainPageViewModel(_fileSystemController);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }

            NavigationService.Navigate(newPage);
        }


    }
}
