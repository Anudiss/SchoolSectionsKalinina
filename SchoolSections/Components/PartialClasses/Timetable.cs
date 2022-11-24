using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSections.DatabaseConnection
{
    public partial class Timetable
    {
        public void Delete()
        {
            if (IsDeleted == true)
                return;

            IsDeleted = true;
            var attendances = from attendance in DatabaseContext.Entities.Attendance.Local
                              where attendance.IsDeleted != true && attendance.Timetable == this
                              select attendance;

            foreach (var attendance in attendances)
                attendance.Delete();
        }
    }
}
