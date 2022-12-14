using SchoolSections.Components.PartialClasses;
using SchoolSections.DatabaseConnection;
using SchoolSections.Permissions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace SchoolSections.Windows.MainWindow.Pages.GroupResources
{
    /// <summary>
    /// Логика взаимодействия для GroupEditWindow.xaml
    /// </summary>
    public partial class GroupEditWindow : Window
    {
        #region Manager
        public Manager Manager
        {
            get { return (Manager)GetValue(ManagerProperty); }
            set
            {
                SetValue(ManagerProperty, value);
                InitializeAttendances();
            }
        }

        // Using a DependencyProperty as the backing store for Manager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManagerProperty =
            DependencyProperty.Register("Manager", typeof(Manager), typeof(GroupEditWindow));
        #endregion
        #region Students
        public ObservableCollection<Student> Students
        {
            get { return (ObservableCollection<Student>)GetValue(StudentsProperty); }
            set { SetValue(StudentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Students.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StudentsProperty =
            DependencyProperty.Register("Students", typeof(ObservableCollection<Student>), typeof(GroupEditWindow));
        #endregion
        #region Attendance
        public ObservableCollection<FullAttendance> Attendance
        {
            get { return (ObservableCollection<FullAttendance>)GetValue(AttendanceProperty); }
            set { SetValue(AttendanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Attendance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttendanceProperty =
            DependencyProperty.Register("Attendance", typeof(ObservableCollection<FullAttendance>), typeof(GroupEditWindow));
        #endregion
        #region Month
        public int Month
        {
            get { return MonthSelector.SelectedIndex + 1; }
            set { MonthSelector.SelectedIndex = value - 1; }
        }
        #endregion
        #region Year
        public int Year
        {
            get { return (int)YearSelector.SelectedItem; }
            set { YearSelector.SelectedItem = value; }
        }
        #endregion

        public GroupEditWindow()
        {
            InitializeComponent();

            MonthSelector.ItemsSource = DateTimeFormatInfo.CurrentInfo.MonthNames.Where(month => month != string.Empty);
            Month = DateTime.Today.Month;

            int minYear = DatabaseContext.Entities.Attendance.Where(a => a.IsDeleted != true).Count() == 0 ? DateTime.Today.Year : DatabaseContext.Entities.Attendance.Where(a => a.IsDeleted != true).Min(attendance => attendance.Date.Year);
            int deltaYear = DateTime.Today.Year - minYear;
            YearSelector.ItemsSource = Enumerable.Range(minYear, deltaYear + 1);
            YearSelector.SelectedIndex = deltaYear;

            Students = new ObservableCollection<Student>(DatabaseContext.Entities.Student.Local.Where(s => s.IsDeleted != true));
        }

        private void InitializeAttendances()
        {
            DateTime now = DateTime.Now;

            var attendances = from a in DatabaseContext.Entities.Attendance.Local
                              where a.Timetable.Manager == Manager &&
                                    a.Timetable.Time != null &&
                                    a.IsDeleted != true
                              select a;

            var timetables = from timetable in DatabaseContext.Entities.Timetable.Local
                             where timetable.Manager == Manager &&
                                   timetable.Time != null &&
                                   timetable.IsDeleted != true
                             select timetable;

            var students = from sm in DatabaseContext.Entities.Student_manager.Local
                           where sm.Manager == Manager &&
                                 sm.IsDeleted != true
                           select sm.Student;

            if (attendances.Count() == 0)
            {
                if (timetables.Count() == 0)
                    return;
                System.DayOfWeek weekDay = Enum.GetValues(typeof(System.DayOfWeek)).Cast<System.DayOfWeek>()
                                                                                   .Reverse()
                                                                                   .First(day => timetables.Any(t => t.DayOfWeek.WeekDay == day) == true);

                Timetable timetable = timetables.First(t => t.DayOfWeek.WeekDay == weekDay);
                if (timetable == null)
                    return;
                
                long ticks = DateTime.Now.Ticks - TimeSpan.TicksPerDay * Math.Abs((int)DateTime.Now.DayOfWeek - (int)weekDay);
                Attendance attendance;
                DatabaseContext.Entities.Attendance.Add(attendance = new Attendance()
                {
                    Timetable = timetable,
                    Date = new DateTime(ticks),
                });

                foreach (var student in students)
                    attendance.Attendance_students.Add(new Attendance_students()
                    {
                        Attendance = attendance,
                        Student = student,
                        IsAttented = true
                    });
                return;
            }

            var lastDate = attendances.Max(a => a.Date);

            var deltaDays = (now - lastDate).Days;
            for (DateTime today = lastDate; today <= DateTime.Now; today += new TimeSpan(1, 0, 0, 0))
            {
                var timetable = timetables.FirstOrDefault(t => t.DayOfWeek.WeekDay == today.DayOfWeek);
                if (timetable == null)
                    continue;

                if (attendances.Any(a => a.Date == today && a.Timetable == timetable))
                    continue;

                Attendance newAttendance;
                DatabaseContext.Entities.Attendance.Add(newAttendance = new Attendance()
                {
                    Timetable = timetable,
                    Date = today
                });

                foreach (var student in students)
                    newAttendance.Attendance_students.Add(new Attendance_students()
                    {
                        Attendance = newAttendance,
                        Student = student,
                        IsAttented = true
                    });
            }
        }

        private void InitializeDataGrid()
        {
            AttendanceContainer.Columns.Clear();
            AttendanceContainer.Columns.Add(new DataGridTextColumn()
            {
                Header = "Ученик",
                IsReadOnly = true,
                Binding = new Binding("StudentManager.Student.FullName")
            });

            IEnumerable<FullAttendance> attendances = from student_manager in DatabaseContext.Entities.Student_manager.Local
                                                      where student_manager.Manager == Manager && student_manager.Student.IsDeleted != true &&
                                                            student_manager.IsDeleted != true
                                                      select new FullAttendance()
                                                      {
                                                          StudentManager = student_manager,
                                                          Month = Month,
                                                          Year = Year
                                                      };

            if (attendances.Count() != 0)
            {
                List<DateTime> dates = attendances.First().Attendances.Select(a => a.Date).ToList();
                for (int i = 0; i < dates.Count; i++)
                {
                    AttendanceContainer.Columns.Add(new DataGridTemplateColumn()
                    {
                        Header = $"{dates[i].Day}",
                        CellTemplate = new DataTemplate()
                        {
                            DataType = typeof(FullAttendance),
                            VisualTree = CreateCheckBox(i)
                        }
                    });
                }
            }
            Attendance = new ObservableCollection<FullAttendance>(attendances);
        }

        private FrameworkElementFactory CreateCheckBox(int index)
        {
            FrameworkElementFactory frameworkElement = new FrameworkElementFactory(typeof(CheckBox));
            frameworkElement.SetValue(ToggleButton.IsThreeStateProperty, true);
            frameworkElement.SetValue(IsEnabledProperty, Permission.MakeAttendance.Has());
            frameworkElement.SetBinding(ToggleButton.IsCheckedProperty, new Binding($"Attendances[{index}].IsAttented")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            return frameworkElement;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeDataGrid();
        }

        private void OnMonthChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Manager != null)
                InitializeDataGrid();
        }

        private void OnYearChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Manager != null)
                InitializeDataGrid();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            foreach (var attendance in Attendance)
            {
                foreach (var attend in attendance.Attendances)
                {
                    System.DayOfWeek weekDay = attend.Date.DayOfWeek;
                    Timetable timetable = attendance.StudentManager.Manager.Timetable.First(t => t.DayOfWeek.WeekDay == weekDay);
                    Attendance dbAttendance = DatabaseContext.Entities.Attendance.Local.FirstOrDefault(a => a.Timetable == timetable && a.Date == attend.Date);
                    if (dbAttendance == null)
                        DatabaseContext.Entities.Attendance.Add(dbAttendance = new Attendance()
                        {
                            Date = attend.Date,
                            Timetable = timetable
                        });

                    dbAttendance.Attendance_students.First(ast => ast.Student == attendance.StudentManager.Student).IsAttented = attend.IsAttented;
                }
            }

            DatabaseContext.Entities.SaveChanges();
        }
    }
}
