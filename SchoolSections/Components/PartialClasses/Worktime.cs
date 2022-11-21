using System;
using System.Linq;

namespace SchoolSections.DatabaseConnection
{
    public partial class Worktime
    {
        public static TimeSpan CurrentStartTime => 
            DatabaseContext.Entities.Worktime.First(worktime => worktime.DayOfWeek.WeekDay == DateTime.Now.DayOfWeek)
                                             .StartTime;

        public static TimeSpan CurrentEndTime =>
            DatabaseContext.Entities.Worktime.First(worktime => worktime.DayOfWeek.WeekDay == DateTime.Now.DayOfWeek)
                                             .EndTime;
    }
}
