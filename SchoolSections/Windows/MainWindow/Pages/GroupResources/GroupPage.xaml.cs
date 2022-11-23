using SchoolSections.DatabaseConnection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSections.Windows.MainWindow.Pages.GroupResources
{
    /// <summary>
    /// Логика взаимодействия для GroupPage.xaml
    /// </summary>
    public partial class GroupPage : Page
    {
        #region Managers
        public ObservableCollection<Manager> Managers
        {
            get { return (ObservableCollection<Manager>)GetValue(ManagersProperty); }
            set { SetValue(ManagersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Managers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManagersProperty =
            DependencyProperty.Register("Managers", typeof(ObservableCollection<Manager>), typeof(GroupPage));
        #endregion

        public GroupPage()
        {
            InitializeComponent();

            Managers = DatabaseContext.Entities.Manager.Local;
        }

        public void OnAttendanceClick(Manager manager)
        {
            new GroupEditWindow()
            {
                Manager = manager
            }.ShowDialog();
        }
    }
}
