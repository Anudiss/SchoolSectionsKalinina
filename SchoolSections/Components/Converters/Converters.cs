using SchoolSections.DatabaseConnection;
using SchoolSections.Permissions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SchoolSections.Components.Converters
{
    #region Image converter
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
    #endregion
    #region Teacher name converter
    public class TeacherNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (SessionData.AuthorizatedUser.PermissionRole == PermissionRole.Director)
                return "Заместитель директора";

            Teacher teacher = SessionData.AuthorizatedUser.SingleTeacher;
            return teacher == null ? "" : $"{teacher.Surname} {teacher.Name} {teacher.Patronymic}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    #endregion
    #region FIOConverter
    public class FIOConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Teacher teacher)
                return $"{teacher.Surname} {teacher.Name} {teacher.Patronymic}";
            if (value is Student student)
                return $"{student.Surname} {student.Name} {student.Partonymic}";
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    #endregion
    #region TimetableByDayConverter
    public class TimetableByDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Timetable timetable == false)
                return null;

            System.DayOfWeek day = (System.DayOfWeek)parameter;
            return timetable.DayOfWeek.WeekDay == day ? timetable?.Time : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    #endregion
    #region SectionToTimetableConverter
    public class SectionToTimetableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Section section = value as Section;

            DatabaseContext.Entities.Timetable.Load();
            return new ObservableCollection<Timetable>(from timetable in DatabaseContext.Entities.Timetable.Local
                                                       where timetable.Manager.Section_id == section.Id_section
                                                       select timetable);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    #endregion
    #region ExpectStudentConverter
    [ValueConversion(typeof(Manager), typeof(ObservableCollection<Student>))]
    public class ExpectStudentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Manager manager == false)
                return null;

            var students = from student_manager in DatabaseContext.Entities.Student_manager.Local
                           where student_manager.Manager == manager
                           select student_manager.Student;

            IEnumerable<Student> excepted = DatabaseContext.Entities.Student.Local.Except(students);
            if (parameter is Student student && student != null)
                excepted.Append(student);
            return excepted.OrderBy(s => s.FullName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
    #endregion

    public static class Extensions
    {
        public static BitmapSource ConvertFromArray(this byte[] data) =>
            (BitmapSource)new ImageSourceConverter().ConvertFrom(data);

        public static byte[] ConvertToArray(this ImageSource source)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            byte[] bytes = null;

            if (source is BitmapSource bitmapSource)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }
    }
}
