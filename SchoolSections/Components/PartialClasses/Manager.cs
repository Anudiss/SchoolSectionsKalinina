using System.Linq;

namespace SchoolSections.DatabaseConnection
{
    public partial class Manager
    {
        public string TeacherSection => $"{Teacher.FullName}-{Section.Name}";

        public void Delete()
        {
            if (IsDeleted == true)
                return;

            IsDeleted = true;

            var timetables = from timetable in DatabaseContext.Entities.Timetable.Local
                             where timetable.IsDeleted != true && timetable.Manager == this
                             select timetable;

            foreach (var timetable in timetables)
                timetable.Delete();

            var studentManager = from studentmanager in DatabaseContext.Entities.Student_manager.Local
                                 where studentmanager.IsDeleted != true && studentmanager.Manager == this
                                 select studentmanager;

            foreach (var studentmanager in studentManager)
                studentmanager.Delete();
        }
    }
}
