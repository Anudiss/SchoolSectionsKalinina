using SchoolSections.DatabaseConnection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Section = SchoolSections.DatabaseConnection.Section;

namespace SchoolSections.Windows.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для SectionPage.xaml
    /// </summary>
    public partial class SectionPage : Page
    {
        #region Sections
        public ObservableCollection<Section> Sections
        {
            get { return (ObservableCollection<Section>)GetValue(SectionsProperty); }
            set { SetValue(SectionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionsProperty =
            DependencyProperty.Register("Sections", typeof(ObservableCollection<Section>), typeof(SectionPage));
        #endregion

        public SectionPage()
        {
            InitializeComponent();

            Sections = new ObservableCollection<Section>(DatabaseContext.Entities.Section);
        }
    }
}
