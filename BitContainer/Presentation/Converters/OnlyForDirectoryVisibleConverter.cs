using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using BitContainer.Presentation.Models;

namespace BitContainer.Presentation.Converters
{
    [ValueConversion(typeof(IAccessWrapperUiModel), typeof(Visibility))]
    public class OnlyForDirectoryVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IAccessWrapperUiModel accessModel)
            {
                if (accessModel.Entity is CDirectoryUiModel)
                    return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
