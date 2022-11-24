using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSections.DatabaseConnection
{
    public partial class Student_manager
    {
        public void Delete()
        {
            if (IsDeleted == true)
                return;

            IsDeleted = true;

            var attendance_students = from attendance_student in DatabaseContext.Entities.Attendance_students.Local
                                      where attendance_student.IsDeleted != true && attendance_student.Student == Student
                                      select attendance_student;

            foreach (var attendance_student in attendance_students)
                attendance_student.Delete();
        }
    }
}
