using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BitContainer.Presentation.ViewModels.Dialogs;

namespace BitContainer.Presentation.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for MoveDestinationDialog.xaml
    /// </summary>
    public partial class MoveDestinationDialog : Window
    {
        public MoveDestinationDialog()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MoveDestinationDialogViewModel vm = (MoveDestinationDialogViewModel) this.DataContext;

            if (vm.Path.CurrentPath.Count > 0)
                DialogResult = true;
        }
    }
}
