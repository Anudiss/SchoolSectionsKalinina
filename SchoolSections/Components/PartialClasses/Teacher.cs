using SchoolSections.Components.Converters;
using SchoolSections.Components.PartialClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace SchoolSections.DatabaseConnection
{
    public partial class Teacher
    {
        public string FullName => $"{Surname} {Name[0]}. {Patronymic[0]}.";

        public BitmapSource Image
        {
            get
            {
                try
                {
                    return TeacherImage.Image_Data.ConvertFromArray();
                }
                catch (Exception)
                {
                    return DatabaseConnection.Image.GetImageByName("noteacherphoto").BitmapImage;
                }
            }
        }

        public IEnumerable<FullTimetable> Timetables
        {
            get => (from timetable in DatabaseContext.Entities.Timetable.Local
                    where timetable.Manager.Teacher == this && timetable.IsDeleted != true
                    select timetable.Manager)
                    .Distinct()
                    .Select(manager => new FullTimetable() { Manager = manager });
        }

        public void Delete()
        {
            if (IsDeleted == true)
                return;

            IsDeleted = true;
            foreach (var manager in DatabaseContext.Entities.Manager.Local.Where(m => m.IsDeleted != true && m.Teacher == this))
                manager.Delete();
        }
    }
}
