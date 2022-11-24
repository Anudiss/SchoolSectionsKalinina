using SchoolSections.DatabaseConnection;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSections.Windows.MainWindow.Pages.TeacherResources
{
    /// <summary>
    /// Логика взаимодействия для TeacherPage.xaml
    /// </summary>
    public partial class TeacherPage : Page
    {
        #region Teachers
        public ObservableCollection<Teacher> Teachers
        {
            get { return (ObservableCollection<Teacher>)GetValue(TeachersProperty); }
            set { SetValue(TeachersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TeachersProperty =
            DependencyProperty.Register("Teachers", typeof(ObservableCollection<Teacher>), typeof(TeacherPage));
        #endregion

        public TeacherPage()
        {
            InitializeComponent();

            DatabaseContext.Entities.Teacher.Load();
            Teachers = new ObservableCollection<Teacher>(DatabaseContext.Entities.Teacher.Local.Where(t => t.IsDeleted != true));
        }

        private void OnSearch(object sender, TextChangedEventArgs e)
        {
            TeacherContainer.Items.Filter = (obj) =>
            {
                Teacher teacher = obj as Teacher;
                string searchingText = SearchComponent.Text.ToLower().Trim();
                return teacher.Surname.Trim().ToLower().StartsWith(searchingText) ||
                       teacher.Name.Trim().ToLower().StartsWith(searchingText) ||
                       teacher.Patronymic.ToLower().Trim().StartsWith(searchingText);
            };
        }

        public void OnTeacherEdit(Teacher teacher)
        {
            new TeacherEditWindow()
            {
                Teacher = teacher
            }.ShowDialog();
        }

        public void OnTeacherRemove(Teacher teacher)
        {
            teacher.Delete();
            Teachers.Remove(teacher);
            DatabaseContext.Entities.SaveChanges();
        }
    }
}
