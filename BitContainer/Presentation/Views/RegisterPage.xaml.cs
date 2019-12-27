using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BitContainer.Presentation.ViewModels;

namespace BitContainer.Presentation.Views
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
