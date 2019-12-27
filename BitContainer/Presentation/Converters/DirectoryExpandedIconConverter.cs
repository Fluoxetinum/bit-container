using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BitContainer.Presentation.Converters
{
    [ValueConversion(typeof(Boolean), typeof(BitmapImage))]
    public class DirectoryExpandedIconConverter : IValueConverter
    {
        private static readonly Uri DirIconUri = 
            new Uri(App.GetPackPath("Icons/folder-48.png"));
        private static readonly Uri OpenDirIconUri = 
            new Uri(App.GetPackPath("Icons/folder-open-48.png"));

        private static readonly BitmapImage DirIcon = new BitmapImage(DirIconUri);
        private static readonly BitmapImage OpenDirIcon = new BitmapImage(OpenDirIconUri);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Boolean expanded)) return DirIcon;
            if (expanded) return OpenDirIcon;
            return DirIcon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
