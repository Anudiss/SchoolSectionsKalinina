using System.Linq;
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
}
