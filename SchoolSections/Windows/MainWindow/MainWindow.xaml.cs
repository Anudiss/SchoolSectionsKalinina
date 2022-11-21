using SchoolSections.DatabaseConnection;
using SchoolSections.Permissions;
using SchoolSections.Windows.MainWindow.Pages;
using SchoolSections.Windows.MainWindow.Pages.SectionResources;
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

            PageContainer.Navigate(new SectionPage());
        }

        private void OnUserChange(object sender, RoutedEventArgs e)
        {
            new AuthWindow.AuthWindow().Show();
            Close();
        }

        private void OnSectionClick(object sender, RoutedEventArgs e) =>
            PageContainer.Navigate(new SectionPage());

        private void OnTeacherClick(object sender, RoutedEventArgs e)
        {
            PageContainer.Navigate(new TeacherPage());
        }
    }
}
