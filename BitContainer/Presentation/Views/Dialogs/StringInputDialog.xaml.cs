using System;
using System.Windows;

namespace BitContainer.Presentation.Views
{
    /// <summary>
    /// Interaction logic for StringInputDialog.xaml
    /// </summary>
    public partial class StringInputDialog : Window
    {
        private String _message;
        public String Message
        {
            get => $"{_message} : ";
            set => _message = value;
        } 

        public String InputField { get; set; }

        public StringInputDialog(String message, String firstInput = "")
        {
            Message = message;
            InitializeComponent();
            DirNameBox.Text = firstInput;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            InputField = DirNameBox.Text;
            DialogResult = !InputField.Equals(String.Empty);
        }
    }
}
