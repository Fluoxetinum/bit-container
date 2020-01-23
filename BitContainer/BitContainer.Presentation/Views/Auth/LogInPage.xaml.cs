using System.Windows;
using System.Windows.Controls;
using BitContainer.Presentation.ViewModels.Auth;

namespace BitContainer.Presentation.Views.Auth
{
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        public LogInPage()
        {
            InitializeComponent();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            LogInPageViewModel vm = (LogInPageViewModel) this.DataContext;
            vm.Password = ((PasswordBox) sender).Password;
        }
    }
}
