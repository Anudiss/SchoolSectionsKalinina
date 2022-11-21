using SchoolSections.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Section = SchoolSections.DatabaseConnection.Section;

namespace SchoolSections.Windows.MainWindow.Pages.TeacherResources
{
    /// <summary>
    /// Логика взаимодействия для TeacherCard.xaml
    /// </summary>
    public partial class TeacherCard : UserControl
    {
        #region Teacher
        public Teacher Teacher
        {
            get { return (Teacher)GetValue(TeacherProperty); }
            set { SetValue(TeacherProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Teacher.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TeacherProperty =
            DependencyProperty.Register("Teacher", typeof(Teacher), typeof(TeacherCard));
        #endregion

        public event Action<Teacher> OnEdit;
        public event Action<Teacher> OnRemove;

        public TeacherCard()
        {
            InitializeComponent();

        }

        private void OnEditClick(object sender, RoutedEventArgs e) =>
            OnEdit?.Invoke(Teacher);

        private void OnRemoveClick(object sender, RoutedEventArgs e) =>
            OnRemove?.Invoke(Teacher);
    }
}
