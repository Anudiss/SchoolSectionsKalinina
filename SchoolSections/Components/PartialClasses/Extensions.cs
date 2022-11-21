using SchoolSections.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolSections.Components.PartialClasses
{
    public class FullTimetable
    {
        private Manager manager;

        public Manager Manager
        {
            get => manager;
            set
            {
                manager = value;
                UpdateTimes();
            }
        }

        public Dictionary<System.DayOfWeek, TimeSpan?> Times { get; private set; }

        public TimeSpan? this[System.DayOfWeek dayOfWeek]
        {
            get => Times?[dayOfWeek];
            set => Times[dayOfWeek] = value;
        }

        public TimeSpan? Monday
        {
            get => this[System.DayOfWeek.Monday];
            set => this[System.DayOfWeek.Monday] = value;
        }
        public TimeSpan? Tuesday
        {
            get => this[System.DayOfWeek.Tuesday];
            set => this[System.DayOfWeek.Tuesday] = value;
        }
        public TimeSpan? Wednesday
        {
            get => this[System.DayOfWeek.Wednesday];
            set => this[System.DayOfWeek.Wednesday] = value;
        }
        public TimeSpan? Thursday
        {
            get => this[System.DayOfWeek.Thursday];
            set => this[System.DayOfWeek.Thursday] = value;
        }
        public TimeSpan? Friday
        {
            get => this[System.DayOfWeek.Friday];
            set => this[System.DayOfWeek.Friday] = value;
        }
        public TimeSpan? Saturday
        {
            get => this[System.DayOfWeek.Saturday];
            set => this[System.DayOfWeek.Saturday] = value;
        }
        public TimeSpan? Sunday
        {
            get => this[System.DayOfWeek.Sunday];
            set => this[System.DayOfWeek.Sunday] = value;
        }
        public decimal Cabinet { get; set; }

        private void UpdateTimes()
        {
            Times = new Dictionary<System.DayOfWeek, TimeSpan?>()
            {
                { System.DayOfWeek.Monday, GetWeekDayTime(System.DayOfWeek.Monday) },
                { System.DayOfWeek.Tuesday, GetWeekDayTime(System.DayOfWeek.Tuesday) },
                { System.DayOfWeek.Wednesday, GetWeekDayTime(System.DayOfWeek.Wednesday) },
                { System.DayOfWeek.Thursday, GetWeekDayTime(System.DayOfWeek.Thursday) },
                { System.DayOfWeek.Friday, GetWeekDayTime(System.DayOfWeek.Friday) },
                { System.DayOfWeek.Saturday, GetWeekDayTime(System.DayOfWeek.Saturday) },
                { System.DayOfWeek.Sunday, GetWeekDayTime(System.DayOfWeek.Sunday) }
            };

            Cabinet = Manager.Section?.Cabinet.Number ?? 0;
        }

        public IEnumerable<System.DayOfWeek> GetTimeConflicts(IEnumerable<FullTimetable> timetables)
        {
            return (from timetable in timetables
                    where timetable != this
                    select LessonTimeIntersection(timetable, this))
                    .SelectMany(e => e)
                    .Distinct();
        }

        private IEnumerable<System.DayOfWeek> LessonTimeIntersection(FullTimetable timetable1, FullTimetable timetable2)
        {
            return from dayOfWeek in Enum.GetValues(typeof(System.DayOfWeek)).Cast<System.DayOfWeek>()
                   where (timetable1.Manager.Section != null && timetable2.Manager.Section != null) && 
                         IsLessonTimeIntersect((timetable1[dayOfWeek], timetable1.Manager.Section.Duration),
                                              (timetable2[dayOfWeek], timetable2.Manager.Section.Duration))
                   select dayOfWeek;
        }

        private bool IsLessonTimeIntersect((TimeSpan? beginTime, TimeSpan? duration) lesson1, (TimeSpan? beginTime, TimeSpan? duration) lesson2)
        {
            return (lesson1.beginTime >= lesson2.beginTime && 
                    lesson1.beginTime <= lesson2.beginTime + lesson2.duration) ||
                   (lesson2.beginTime >= lesson1.beginTime &&
                    lesson2.beginTime <= lesson1.beginTime + lesson1.duration);
            /*
             * lesson1 = (5:00, 1:30)
             * lesson2 = (5:30, 1:30)
             * result = 5:00 >= 5:30 && 5:30 <= 7:00
             * result = 5:30 >= 5:00 && 5:30 <= 6:30
             */
        }

        private TimeSpan? GetWeekDayTime(System.DayOfWeek day)
        {
            return (from timetable in DatabaseContext.Entities.Timetable.Local
                    where timetable.Manager == Manager &&
                          timetable.DayOfWeek.WeekDay == day &&
                          timetable.IsDeleted != true
                    select timetable.Time).FirstOrDefault();
        }
    }
}
