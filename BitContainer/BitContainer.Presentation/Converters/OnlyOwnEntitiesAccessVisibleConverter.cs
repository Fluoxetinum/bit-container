using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.Converters
{
    [ValueConversion(typeof(FileSystemNode), typeof(Visibility))]
    public class OnlyOwnEntitiesAccessVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is FileSystemNode node)) return Visibility.Visible;
            if (node.IsSharedWithUser) return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
