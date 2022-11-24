using SchoolSections.DatabaseConnection;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSections.Windows.MainWindow.Pages.StatisticsResources
{
    /// <summary>
    /// Логика взаимодействия для StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        #region Students
        public ObservableCollection<StudentAttendanceStat> Students
        {
            get { return (ObservableCollection<StudentAttendanceStat>)GetValue(StudentsProperty); }
            set { SetValue(StudentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StudentsProperty =
            DependencyProperty.Register("Students", typeof(ObservableCollection<StudentAttendanceStat>), typeof(StatisticsPage));
        #endregion
        #region SectionAttendance
        public ObservableCollection<SectionAttendanceStat> SectionAttendance
        {
            get { return (ObservableCollection<SectionAttendanceStat>)GetValue(SectionAttendanceProperty); }
            set { SetValue(SectionAttendanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionAttendanceProperty =
            DependencyProperty.Register("SectionAttendance", typeof(ObservableCollection<SectionAttendanceStat>), typeof(StatisticsPage));
        #endregion

        public int Month
        {
            get => MonthSelector.SelectedIndex;
            set => MonthSelector.SelectedIndex = value;
        }

        public int Year
        {
            get => (int)YearSelector.SelectedItem;
            set => YearSelector.SelectedIndex = value;
        }

        public StatisticsPage()
        {
            InitializeComponent();

            MonthSelector.ItemsSource = DateTimeFormatInfo.CurrentInfo.MonthNames.Where(month => month != string.Empty);
            Month = DateTime.Today.Month;

            int minYear = DatabaseContext.Entities.Attendance.Where(a => a.IsDeleted != true).Count() == 0 ? DateTime.Today.Year : DatabaseContext.Entities.Attendance.Where(a => a.IsDeleted != true).Min(attendance => attendance.Date.Year);
            int deltaYear = DateTime.Today.Year - minYear;
            YearSelector.ItemsSource = Enumerable.Range(minYear, deltaYear + 1);
            YearSelector.SelectedIndex = deltaYear;

            Students = new ObservableCollection<StudentAttendanceStat>(
                    (from student in DatabaseContext.Entities.Student.Local
                     where student.IsDeleted != true
                     select new StudentAttendanceStat()
                     {
                         Student = student,
                         AttendanceCount = student.Attendance_students.Count(s => s.IsAttented == true)
                     }).Where(s => s.AttendanceCount >= 0)
                     .OrderByDescending(s => s.AttendanceCount).ThenBy(s => s.Student.FullName).Take(10)
                );

            
        }

        private void UpdateStatistics()
        {

        }

        private void UpdateBestStudents()
        {

        }

        private void UpdateBestSections()
        {
            SectionAttendance = new ObservableCollection<SectionAttendanceStat>(
                    (from attendance_student in DatabaseContext.Entities.Attendance_students.Local
                     join attendance in DatabaseContext.Entities.Attendance.Local on attendance_student.Attendance equals attendance
                     where attendance_student.Attendance.Date <= new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month)) &&
                           attendance_student.IsDeleted != true && attendance_student.Student.IsDeleted != true
                     select attendance).GroupBy(a => a.Timetable.Manager.Section)
                                       .Select(a => new SectionAttendanceStat()
                                       {
                                           Section = a.Key,
                                           AttendanceCount = a.Count()
                                       }).OrderByDescending(a => a.AttendanceCount).ThenBy(a => a.Section.Name).Take(10)
                );
        }
    }

    public class SectionAttendanceStat
    {
        public Section Section { get; set; }
        public int AttendanceCount { get; set; }
    }

    public class StudentAttendanceStat
    {
        public Student Student { get; set; }
        public int AttendanceCount { get; set; }
    }
}
