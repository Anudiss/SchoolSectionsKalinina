using SchoolSections.Components.Converters;
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
                    return Image_Data.ConvertFromArray();
                }
                catch(Exception)
                {
                    return DatabaseConnection.Image.GetImageByName("Noimage").BitmapImage;
                }
            }
        }
    }
}
