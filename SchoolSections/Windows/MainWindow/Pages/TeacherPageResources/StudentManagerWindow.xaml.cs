using SchoolSections.DatabaseConnection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSections.Windows.MainWindow.Pages.TeacherPageResources
{
    /// <summary>
    /// Логика взаимодействия для StudentManagerWindow.xaml
    /// </summary>
    public partial class StudentManagerWindow : Window
    {
        #region Manager
        public Manager Manager
        {
            get { return (Manager)GetValue(ManagerProperty); }
            set
            {
                SetValue(ManagerProperty, value);
                StudentManagers = new ObservableCollection<Student_manager>(from student_manager in DatabaseContext.Entities.Student_manager.Local
                                                                            where student_manager.Manager == value
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
        #endregion

        public StudentManagerWindow()
        {
            InitializeComponent();
            Students = new ObservableCollection<Student>(DatabaseContext.Entities.Student.Local.OrderBy(student => student.FullName));
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
                    (e.Row.Item as Student_manager).Student = student;
                }
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
            var exceptedStudents = Students.Except(from student_manager in DatabaseContext.Entities.Student_manager.Local
                                                   where student_manager.Manager == Manager
                                                   select student_manager.Student);

            if (!e.Row.IsNewItem)
                exceptedStudents.Append((StudentManagerContainer.Items[StudentManagerContainer.SelectedIndex] as Student_manager).Student);
            OtherStudents = new ObservableCollection<Student>(exceptedStudents);
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            foreach (var studentManager in StudentManagers)
            {
                if (DatabaseContext.Entities.Student_manager.Any(student_Manager => student_Manager.Id_student_section == studentManager.Id_student_section) == false)
                    DatabaseContext.Entities.Student_manager.Add(studentManager);
            }
            DatabaseContext.Entities.SaveChanges();
            Close();
        }
    }
}
