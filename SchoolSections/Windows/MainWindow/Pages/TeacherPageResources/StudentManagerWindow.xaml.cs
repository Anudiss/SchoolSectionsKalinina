using SchoolSections.DatabaseConnection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SchoolSections.Windows.MainWindow.Pages.TeacherPageResources
{
    /// <summary>
    /// Логика взаимодействия для StudentManagerWindow.xaml
    /// </summary>
    public partial class StudentManagerWindow : Window
    {
        public static RoutedCommand RemoveCommand
        {
            get => removeCommand ?? (removeCommand = new RoutedCommand());
            set => removeCommand = value;
        }

        #region Manager
        public Manager Manager
        {
            get { return (Manager)GetValue(ManagerProperty); }
            set
            {
                SetValue(ManagerProperty, value);
                StudentManagers = new ObservableCollection<Student_manager>(from student_manager in DatabaseContext.Entities.Student_manager.Local
                                                                            where student_manager.Manager == value &&
                                                                                  student_manager.IsDeleted != true
                                                                            select student_manager);
            }
        }

        // Using a DependencyProperty as the backing store for Manager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManagerProperty =
            DependencyProperty.Register("Manager", typeof(Manager), typeof(StudentManagerWindow));
        #endregion
        #region StudentManagers
        public ObservableCollection<Student_manager> StudentManagers
        {
            get { return (ObservableCollection<Student_manager>)GetValue(StudentManagersProperty); }
            set { SetValue(StudentManagersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StudentManagers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StudentManagersProperty =
            DependencyProperty.Register("StudentManagers", typeof(ObservableCollection<Student_manager>), typeof(StudentManagerWindow));
        #endregion
        #region Students
        public ObservableCollection<Student> Students
        {
            get { return (ObservableCollection<Student>)GetValue(StudentsProperty); }
            set { SetValue(StudentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Students.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StudentsProperty =
            DependencyProperty.Register("Students", typeof(ObservableCollection<Student>), typeof(StudentManagerWindow));
        #endregion
        #region OtherStudents
        public ObservableCollection<Student> OtherStudents
        {
            get { return (ObservableCollection<Student>)GetValue(OtherStudentsProperty); }
            set { SetValue(OtherStudentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OtherStudentsProperty =
            DependencyProperty.Register("OtherStudents", typeof(ObservableCollection<Student>), typeof(StudentManagerWindow));
        private static RoutedCommand removeCommand;
        #endregion

        public StudentManagerWindow()
        {
            InitializeComponent();
            Students = new ObservableCollection<Student>(DatabaseContext.Entities.Student.Local.Where(s => s.IsDeleted != true).OrderBy(student => student.FullName));
        }

        private void StudentManagerContainer_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditingElement is ComboBox comboBox)
            {
                Student student = comboBox.SelectedItem as Student;
                Student_manager sm = e.Row.Item as Student_manager;
                if (StudentManagers.Any(studentManager => studentManager != sm && studentManager.Student == student))
                {
                    if (student != null)
                        MessageBox.Show("Этот студент уже в группе");

                    return;
                }

                if (student == null && !e.Row.IsNewItem)
                {
                    comboBox.SelectedItem = student = StudentManagers[e.Row.AlternationIndex].Student;
                }
                if (student != null)
                    (e.Row.Item as Student_manager).Student = student;
            }
        }

        private void StudentManagerContainer_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new Student_manager()
            {
                Manager = Manager
            };
        }

        private void StudentManagerContainer_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (!e.Row.IsNewItem)
                e.Cancel = true;

            var exceptedStudents = Students.Except(from student_manager in DatabaseContext.Entities.Student_manager.Local
                                                   where student_manager.Manager == Manager && student_manager.IsDeleted != true
                                                   select student_manager.Student);

            if (!e.Row.IsNewItem)
                exceptedStudents.Append((StudentManagerContainer.Items[StudentManagerContainer.SelectedIndex] as Student_manager).Student);
            OtherStudents = new ObservableCollection<Student>(exceptedStudents);
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            foreach (var studentManager in StudentManagers)
            {
                Student_manager student_Manager1 = DatabaseContext.Entities.Student_manager.Local.Where(sm => sm.IsDeleted != true).FirstOrDefault(student_Manager => student_Manager == studentManager);
                if (student_Manager1 == null)
                    DatabaseContext.Entities.Student_manager.Add(student_Manager1 = studentManager);

                var timetables = from timetable in DatabaseContext.Entities.Timetable.Local
                                 where timetable.IsDeleted != true &&
                                       timetable.Manager == studentManager.Manager
                                 select timetable;

                var attendances = from attendance in DatabaseContext.Entities.Attendance.Local
                                  join timetable in timetables on attendance.Timetable equals timetable
                                  where attendance.IsDeleted != true
                                  select attendance;

                foreach (var attendance in attendances.Distinct())
                {
                    if (attendance.Attendance_students.Any(ast => ast.Student == studentManager.Student))
                        continue;

                    attendance.Attendance_students.Add(new Attendance_students()
                    {
                        Attendance = attendance,
                        Student = studentManager.Student,
                        IsAttented = null
                    });
                }
            }
            DatabaseContext.Entities.SaveChanges();
            Close();
        }

        private void RemoveCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Student_manager student_Manager = e.Parameter as Student_manager;
            student_Manager.Delete();
            StudentManagers.Remove(student_Manager);
        }

        private void RemoveCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) =>
                e.CanExecute = e.Parameter is Student_manager;
    }
}
