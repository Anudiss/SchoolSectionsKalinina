using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SchoolSections.DatabaseConnection
{
    public partial class Image
    {
        public static Image GetImageByName(string name) =>
            DatabaseContext.Entities.Image.FirstOrDefault(image => image.Name.ToLower() == name.Trim().ToLower());

        public BitmapSource BitmapImage => (BitmapSource)new ImageSourceConverter().ConvertFrom(Data);
    }

    [ValueConversion(typeof(string), typeof(BitmapSource))]
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string imageName == false)
                return null;

            return Image.GetImageByName(imageName).BitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
