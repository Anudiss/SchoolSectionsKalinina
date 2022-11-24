using System.Linq;

namespace SchoolSections.DatabaseConnection
{
    public partial class Attendance
    {
        public void Delete()
        {
            if (IsDeleted == true)
                return;

            IsDeleted = true;

            var attendanceStudents = from attendance_students in DatabaseContext.Entities.Attendance_students.Local
                                    where attendance_students.IsDeleted != true && attendance_students.Attendance == this
                                    select attendance_students;

            foreach (var attendanceStudent in attendanceStudents)
                attendanceStudent.Delete();

        }
    }
}
