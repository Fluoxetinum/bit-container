using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using BitContainer.Presentation.Models;
using BitContainer.Presentation.ViewModels.Nodes;
using MaterialDesignThemes.Wpf;

namespace BitContainer.Presentation.Converters
{
    [ValueConversion(typeof(FileSystemNode), typeof(PackIconKind))]
    public class StorageEntityMaterialIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var node = value as FileSystemNode;
            if (node == null) return PackIconKind.QuestionMark;

            if (node.Entity is CDirectoryUi) return PackIconKind.Folder;
            else if (node.Entity is CFileUi) return PackIconKind.File;
            return PackIconKind.QuestionMark;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
