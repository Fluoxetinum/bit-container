using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BitContainer.Presentation.Models;
using MaterialDesignThemes.Wpf;

namespace BitContainer.Presentation.Converters
{
    [ValueConversion(typeof(IAccessWrapperUiModel), typeof(BitmapImage))]
    public class StorageEntityIconConverter : IValueConverter
    {
        private static readonly Dictionary<String, Uri> Uries = new Dictionary<string, Uri>()
        {
            [String.Empty] =  new Uri(App.GetPackPath("Icons/folder-96.png")),

            [".txt"] = new Uri(App.GetPackPath("Icons/txt-96.png")),
            [".docx"] = new Uri(App.GetPackPath("Icons/word-96.png")),
            [".png"] = new Uri(App.GetPackPath("Icons/png-96.png")),
            [".jpg"] = new Uri(App.GetPackPath("Icons/jpg-96.png")),
            [".pdf"] = new Uri(App.GetPackPath("Icons/pdf-96.png")),
            [".zip"] = new Uri(App.GetPackPath("Icons/zip-96.png")),
            [".mp3"] = new Uri(App.GetPackPath("Icons/mp3-96.png")),
            [".exe"] = new Uri(App.GetPackPath("Icons/exe-96.png")),
                               
            [".ico"] = new Uri(App.GetPackPath("Icons/image-96.png")),

        };

        private static readonly Uri UnknownIcon = 
            new Uri(App.GetPackPath("Icons/unknown-96.png"));


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var accessModel = value as IAccessWrapperUiModel;
            if (accessModel == null) return PackIconKind.QuestionMark;

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            switch (accessModel.Entity)
            {
                case CDirectoryUiModel dir:
                    image.UriSource = Uries[String.Empty];
                    break;
                case CFileUiModel file:
                    Uries.TryGetValue(file.Extension, out var uri);
                    if (uri == null) uri = UnknownIcon;
                    image.UriSource = uri;
                    break;
            }
            image.EndInit();

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
