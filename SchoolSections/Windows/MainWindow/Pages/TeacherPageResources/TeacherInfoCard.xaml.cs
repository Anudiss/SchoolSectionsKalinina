using SchoolSections.DatabaseConnection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSections.Windows.MainWindow.Pages.TeacherPageResources
{
    /// <summary>
    /// Логика взаимодействия для TeacherInfoCard.xaml
    /// </summary>
    public partial class TeacherInfoCard : UserControl
    {
        #region Manager
        public Manager Manager
        {
            get { return (Manager)GetValue(ManagerProperty); }
            set { SetValue(ManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Manager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManagerProperty =
            DependencyProperty.Register("Manager", typeof(Manager), typeof(TeacherInfoCard));
        #endregion

        public event Action<Manager> OnAttendanceClick;
        public event Action<Manager> OnStudentsClick;

        public TeacherInfoCard()
        {
            InitializeComponent();
        }

        private void AttendanceClick(object sender, RoutedEventArgs e) =>
            OnAttendanceClick?.Invoke(Manager);

        private void StudentsClick(object sender, RoutedEventArgs e) =>
            OnStudentsClick?.Invoke(Manager);
    }
}
