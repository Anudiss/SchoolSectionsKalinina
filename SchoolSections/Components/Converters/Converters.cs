using SchoolSections.DatabaseConnection;
using SchoolSections.Permissions;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SchoolSections.Components.Converters
{

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

    public class TeacherNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (SessionData.AuthorizatedUser.PermissionRole == PermissionRole.Director)
                return "Заместитель директора";

            Teacher teacher = SessionData.AuthorizatedUser.Teacher.First();
            return teacher == null ? "" : $"{teacher.Surname} {teacher.Name} {teacher.Patronymic}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
