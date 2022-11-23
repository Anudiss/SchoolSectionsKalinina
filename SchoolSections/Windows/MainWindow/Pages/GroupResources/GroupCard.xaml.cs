using SchoolSections.DatabaseConnection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSections.Windows.MainWindow.Pages.GroupResources
{
    /// <summary>
    /// Логика взаимодействия для GroupCard.xaml
    /// </summary>
    public partial class GroupCard : UserControl
    {
        #region Manager
        public Manager Manager
        {
            get { return (Manager)GetValue(ManagerProperty); }
            set { SetValue(ManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Manager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManagerProperty =
            DependencyProperty.Register("Manager", typeof(Manager), typeof(GroupCard));
        #endregion

        public event Action<Manager> AttendanceClick;

        public GroupCard()
        {
            InitializeComponent();
        }

        private void OnAttendanceClick(object sender, RoutedEventArgs e) =>
            AttendanceClick?.Invoke(Manager);
    }
}
