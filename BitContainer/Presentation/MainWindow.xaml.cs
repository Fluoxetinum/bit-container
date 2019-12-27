using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Windows.Navigation;
using BitContainer.Contracts.V1;
using BitContainer.Presentation.Controllers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;


namespace BitContainer.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            if (AuthController.AuthenticatedUserUiModel == null)
            {
                NavigationController.GoToLoginPage();
            }
        }
    }
}
