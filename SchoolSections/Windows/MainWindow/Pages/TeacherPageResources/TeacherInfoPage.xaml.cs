using SchoolSections.DatabaseConnection;
using SchoolSections.Permissions;
using SchoolSections.Windows.MainWindow.Pages.GroupResources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSections.Windows.MainWindow.Pages.TeacherPageResources
{
    /// <summary>
    /// Логика взаимодействия для TeacherInfoPage.xaml
    /// </summary>
    public partial class TeacherInfoPage : Page
    {
        #region Managers
        public ObservableCollection<Manager> Managers
        {
            get { return (ObservableCollection<Manager>)GetValue(ManagersProperty); }
            set { SetValue(ManagersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Managers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManagersProperty =
            DependencyProperty.Register("Managers", typeof(ObservableCollection<Manager>), typeof(TeacherInfoPage));
        #endregion
        #region Students
        public ObservableCollection<Student> Students
        {
            get { return (ObservableCollection<Student>)GetValue(StudentsProperty); }
            set { SetValue(StudentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StudentsProperty =
            DependencyProperty.Register("Students", typeof(ObservableCollection<Student>), typeof(TeacherInfoPage));
        #endregion
        #region StudentManagers
        public ObservableCollection<Student_manager> StudentManagers
        {
            get { return (ObservableCollection<Student_manager>)GetValue(StudentManagersProperty); }
            set { SetValue(StudentManagersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StudentManagers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StudentManagersProperty =
            DependencyProperty.Register("StudentManagers", typeof(ObservableCollection<Student_manager>), typeof(TeacherInfoPage));
        #endregion

        public IEnumerable<Student> OtherStudents { get; set; }

        public TeacherInfoPage()
        {
            InitializeComponent();

            Managers = new ObservableCollection<Manager>(from manager in DatabaseContext.Entities.Manager.Local
                                                         where manager.Teacher == SessionData.AuthorizatedUser.SingleTeacher && manager.IsDeleted != true
                                                         select manager);

            StudentManagers = new ObservableCollection<Student_manager>(from student_manager in DatabaseContext.Entities.Student_manager.Local
                                                                        where student_manager.Manager.Teacher == SessionData.AuthorizatedUser.SingleTeacher && student_manager.IsDeleted != true
                                                                        select student_manager);
            Students = new ObservableCollection<Student>(from student_manager in StudentManagers
                                                         where student_manager.IsDeleted != true
                                                         select student_manager.Student);

            OtherStudents = from student in DatabaseContext.Entities.Student.Local
                            where student.IsDeleted != true && Students?.Contains(student) == false
                            select student;

        }

        private void OnAttendanceClick(Manager manager)
        {
            new GroupEditWindow()
            {
                Manager = manager
            }.ShowDialog();
        }

        private void OnStudentsClick(Manager manager)
        {
            new StudentManagerWindow()
            {
                Manager = manager
            }.ShowDialog();
        }
    }
}
