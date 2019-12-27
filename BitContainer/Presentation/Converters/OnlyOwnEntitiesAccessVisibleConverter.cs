using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using BitContainer.Presentation.Models;

namespace BitContainer.Presentation.Converters
{
    [ValueConversion(typeof(IAccessWrapperUiModel), typeof(Visibility))]
    public class OnlyOwnEntitiesAccessVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IAccessWrapperUiModel accessModel)) return Visibility.Visible;

            if (accessModel is CRestrictedStorageEntityUiModel restricted) return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
