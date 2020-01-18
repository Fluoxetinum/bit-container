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
using BitContainer.Presentation.ViewModels.Commands;

namespace BitContainer.Presentation.Views.Controls
{
    /// <summary>
    /// Interaction logic for PathControl.xaml
    /// </summary>
    public partial class PathControl : UserControl
    {
        public PathControl()
        {
            InitializeComponent();
        }

        public static DependencyProperty SelectedEntryCommandProperty = 
            DependencyProperty.Register(nameof(SelectedEntryCommand), 
                typeof(ICommand), 
                typeof(PathControl));

        public ICommand SelectedEntryCommand
        {
            get => (ICommand) GetValue(SelectedEntryCommandProperty);
            set => SetValue(SelectedEntryCommandProperty, value);
        }

    }
}
