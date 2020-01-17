using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using BitContainer.Presentation.Models;
using MaterialDesignThemes.Wpf;

namespace BitContainer.Presentation.Converters
{
    [ValueConversion(typeof(IAccessWrapperUiModel), typeof(PackIconKind))]
    public class StorageEntityMaterialIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var accessModel = value as IAccessWrapperUiModel;
            if (accessModel == null) return PackIconKind.QuestionMark;

            if (accessModel.Entity is CDirectoryUiModel) return PackIconKind.Folder;
            else if (accessModel.Entity is CFileUiModel) return PackIconKind.File;
            return PackIconKind.QuestionMark;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
