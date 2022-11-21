using SchoolSections.Components.PartialClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace SchoolSections.DatabaseConnection
{
    public partial class Section
    {
        public BitmapSource Image
        {
            get
            {
                try
                {
                    return SectionImage.Image;
                }
                catch (NullReferenceException)
                {
                    return DatabaseConnection.Image.GetImageByName("noimage").BitmapImage;
                }
            }
        }

        public IEnumerable<FullTimetable> Timetables
        {
            get => (from timetable in DatabaseContext.Entities.Timetable.Local
                    where timetable.Manager.Section == this && timetable.IsDeleted != true
                    select timetable.Manager)
                    .Distinct()
                    .Select(manager => new FullTimetable() { Manager = manager });
        }
    }
}
