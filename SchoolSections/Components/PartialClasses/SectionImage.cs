using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SchoolSections.DatabaseConnection
{
    public partial class SectionImage
    {
        public BitmapSource Image
        {
            get
            {
                try
                {
                    return (BitmapSource)new ImageSourceConverter().ConvertFrom(Image_Data);
                } catch(NotSupportedException) { return DatabaseConnection.Image.GetImageByName("Noimage").BitmapImage; }
            }
        }
    }
}
