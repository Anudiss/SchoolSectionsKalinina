using SchoolSections.DatabaseConnection;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSections.Windows.MainWindow.Pages.SectionResources
{
    public delegate void ButtonClickHandler(Section section);

    /// <summary>
    /// Логика взаимодействия для SectionCard.xaml
    /// </summary>
    public partial class SectionCard : UserControl
    {
        #region Section
        public Section Section
        {
            get { return (Section)GetValue(SectionProperty); }
            set { SetValue(SectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionProperty =
            DependencyProperty.Register("Section", typeof(Section), typeof(SectionCard));
        #endregion

        public event ButtonClickHandler OnEdit;
        public event ButtonClickHandler OnRemove;

        public SectionCard()
        {
            InitializeComponent();
        }

        private void OnEditClicked(object sender, RoutedEventArgs e) =>
            OnEdit?.Invoke(Section);

        private void OnRemoveClick(object sender, RoutedEventArgs e) =>
            OnRemove?.Invoke(Section);
    }
}
