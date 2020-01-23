using System.Windows;
using System.Windows.Controls;
using BitContainer.Presentation.ViewModels.Auth;

namespace BitContainer.Presentation.Views.Auth
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void OnPasswordConfirmationChanged(object sender, RoutedEventArgs e)
        {
            RegisterPageViewModel model = (RegisterPageViewModel) this.DataContext;
            model.PasswordConfirmation = ((PasswordBox) sender).Password;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            RegisterPageViewModel model = (RegisterPageViewModel) this.DataContext;
            model.Password = ((PasswordBox) sender).Password;
        }
    }
}
