using SchoolSections.DatabaseConnection;
using SchoolSections.Permissions;
using SchoolSections.Windows.MainWindow.Pages;
using SchoolSections.Windows.MainWindow.Pages.GroupResources;
using SchoolSections.Windows.MainWindow.Pages.SectionResources;
using SchoolSections.Windows.MainWindow.Pages.StatisticsResources;
using SchoolSections.Windows.MainWindow.Pages.TeacherPageResources;
using SchoolSections.Windows.MainWindow.Pages.TeacherResources;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SchoolSections.Windows.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (SessionData.AuthorizatedUser.PermissionRole == PermissionRole.Teacher)
                PageContainer.Navigate(new TeacherInfoPage());
            else if (SessionData.AuthorizatedUser.PermissionRole == PermissionRole.Director)
                PageContainer.Navigate(new SectionPage());
        }

        private void OnUserChange(object sender, RoutedEventArgs e)
        {
            new AuthWindow.AuthWindow().Show();
            Close();
        }

        private void OnSectionClick(object sender, RoutedEventArgs e) =>
            PageContainer.Navigate(new SectionPage());

        private void OnTeacherClick(object sender, RoutedEventArgs e) =>
            PageContainer.Navigate(new TeacherPage());

        private void OnGroupsClick(object sender, RoutedEventArgs e) =>
            PageContainer.Navigate(new GroupPage());

        private void OnMoyGroupsClick(object sender, RoutedEventArgs e) =>
            PageContainer.Navigate(new TeacherInfoPage());

        private void OnStatisticsClick(object sender, RoutedEventArgs e) =>
            PageContainer.Navigate(new StatisticsPage());
    }
}
