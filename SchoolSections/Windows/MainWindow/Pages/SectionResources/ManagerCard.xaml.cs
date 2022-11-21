using SchoolSections.DatabaseConnection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSections.Windows.MainWindow.Pages.SectionResources
{
    /// <summary>
    /// Логика взаимодействия для ManagerCard.xaml
    /// </summary>
    public partial class ManagerCard : UserControl
    {
        #region Manager
        public Manager Manager
        {
            get { return (Manager)GetValue(ManagerProperty); }
            set { SetValue(ManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManagerProperty =
            DependencyProperty.Register("Manager", typeof(Manager), typeof(ManagerCard));
        #endregion

        public event Action<Manager> OnManagerDelete;
        public event Action<Manager> OnNavigate;

        public ManagerCard()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) =>
            OnManagerDelete?.Invoke(Manager);

        private void Button_Click_1(object sender, RoutedEventArgs e) =>
            OnNavigate?.Invoke(Manager);
    }
}
