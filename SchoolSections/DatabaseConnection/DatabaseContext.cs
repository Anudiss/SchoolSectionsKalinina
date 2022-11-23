using System.Data.Entity;

namespace SchoolSections.DatabaseConnection
{
    public static class DatabaseContext
    {
        public static SchoolEntities Entities { get; }

        static DatabaseContext()
        {
            Entities = new SchoolEntities();

            Entities.User.Load();
            Entities.Image.Load();
            Entities.Timetable.Load();
            Entities.Section.Load();
            Entities.Teacher.Load();
            Entities.DayOfWeek.Load();
            Entities.Manager.Load();
            Entities.Student.Load();
            Entities.Attendance.Load();
            Entities.Attendance_students.Load();
            Entities.Student_manager.Load();
        }
    }
}
