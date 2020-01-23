using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Nodes;

namespace BitContainer.Presentation.Converters
{
    [ValueConversion(typeof(FileSystemNode), typeof(Visibility))]
    public class OnlyForDirectoryVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileSystemNode node && node.IsDir)
                return Visibility.Visible;
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
