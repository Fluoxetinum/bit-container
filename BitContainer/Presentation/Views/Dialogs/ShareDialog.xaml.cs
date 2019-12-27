using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Enums;

namespace BitContainer.Presentation.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ShareDialog.xaml
    /// </summary>
    public partial class ShareDialog : Window
    {
        public ShareDialog()
        {
            InitializeComponent();
        }

        public String UserName { get; set; }
        public EAccessTypeUiModel Access { get; set; }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            UserName = UserNameBox.Text;
            Access = (EAccessTypeUiModel)AccessTypeBox.SelectedValue;

            DialogResult = UserName != String.Empty;

        }
    }
}
