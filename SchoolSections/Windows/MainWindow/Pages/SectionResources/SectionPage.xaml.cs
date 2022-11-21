using SchoolSections.DatabaseConnection;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using Section = SchoolSections.DatabaseConnection.Section;

namespace SchoolSections.Windows.MainWindow.Pages.SectionResources
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

            DatabaseContext.Entities.Section.Load();
            Sections = DatabaseContext.Entities.Section.Local;
        }

        public void OnSearch(object sender, TextChangedEventArgs e) =>
            SectionContainer.Items.Filter = (obj) => {
                var section = obj as Section;
                return section.Name.Trim().ToLower().StartsWith(SearchComponent?.Text.ToLower().Trim() ?? string.Empty)
                       && section.IsDeleted != true;
            };

        public void OnAddSection(object sender, RoutedEventArgs e)
        {
            new SectionEditWindow()
            {
                Section = null
            }.ShowDialog();
            SectionContainer.Items.Refresh();
        }

        public void OnEditSection(Section section)
        {
            new SectionEditWindow()
            {
                Section = section
            }.ShowDialog();
            SectionContainer.Items.Refresh();
        }

        public void OnRemoveSection(Section section)
        {
            new SectionEditWindow()
            {
                Section = section
            }.ShowDialog();
            SectionContainer.Items.Refresh();
        }
    }
}
