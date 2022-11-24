using Microsoft.Win32;
using SchoolSections.Components.Converters;
using SchoolSections.Components.PartialClasses;
using SchoolSections.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Image = SchoolSections.DatabaseConnection.Image;
using Section = SchoolSections.DatabaseConnection.Section;

namespace SchoolSections.Windows.MainWindow.Pages.TeacherResources
{
    /// <summary>
    /// Логика взаимодействия для TeacherEditWindow.xaml
    /// </summary>
    public partial class TeacherEditWindow : Window
    {
        #region Teacher
        public Teacher Teacher
        {
            get { return (Teacher)GetValue(TeacherProperty); }
            set
            {
                SetValue(TeacherProperty, value);
                if (value != null)
                {
                    Timetables = new ObservableCollection<FullTimetable>(value.Timetables);
                }
                else
                {
                    Timetables = new ObservableCollection<FullTimetable>();
                }
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TeacherProperty =
            DependencyProperty.Register("Teacher", typeof(Teacher), typeof(TeacherEditWindow));

        #endregion
        #region Timetables
        public ObservableCollection<FullTimetable> Timetables
        {
            get { return (ObservableCollection<FullTimetable>)GetValue(TimetablesProperty); }
            set
            {
                SetValue(TimetablesProperty, value);
                Sections = new ObservableCollection<Section>(DatabaseContext.Entities.Section.Local.Where(s => s.IsDeleted != true));
                SectionsView = new CollectionView(Sections)
                {
                    Filter = (section) => !value.Any(timetable => timetable.Manager.Section == section)
                };
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimetablesProperty =
            DependencyProperty.Register("Timetables", typeof(ObservableCollection<FullTimetable>), typeof(TeacherEditWindow));
        #endregion
        #region Sections
        public ObservableCollection<Section> Sections
        {
            get { return (ObservableCollection<Section>)GetValue(SectionsProperty); }
            set { SetValue(SectionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Sections.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionsProperty =
            DependencyProperty.Register("Sections", typeof(ObservableCollection<Section>), typeof(TeacherEditWindow));
        #endregion

        public CollectionView SectionsView
        {
            get { return (CollectionView)GetValue(SectionsViewProperty); }
            set { SetValue(SectionsViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SectionsView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionsViewProperty =
            DependencyProperty.Register("SectionsView", typeof(CollectionView), typeof(TeacherEditWindow));

        public TeacherEditWindow()
        {
            InitializeComponent();
        }

        public bool OnValidate(object sender, Components.UserControls.ValidateEventArgs args) =>
            args.Text.Trim().Length > 2;

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (Timetables.Any(timetable => timetable.Manager.Section == null))
            {
                MessageBox.Show("Некоторые кружки не заполнены");
                return;
            }

            if (Timetables.Select(timetable => timetable.Manager.Section).Count() != Timetables.Select(timetable => timetable.Manager.Section).Distinct().Count())
            {
                MessageBox.Show("Учитель не может вести одинаковые кружки");
                return;
            }

            ObservableCollection<Timetable> managerTimetables = new ObservableCollection<Timetable>(DatabaseContext.Entities.Timetable.Local.Where(t => t.IsDeleted != true));
            foreach (var timetable in Timetables)
            {
                IEnumerable<Timetable> tables = managerTimetables.Where(table => table.Manager == timetable.Manager);
                foreach (var weekDay in Enum.GetValues(typeof(System.DayOfWeek)).Cast<System.DayOfWeek>())
                {
                    Timetable currentTimetable = tables.FirstOrDefault(table => table.DayOfWeek_id == (int)weekDay);
                    if (currentTimetable == null)
                        DatabaseContext.Entities.Timetable.Add(currentTimetable = new Timetable()
                        {
                            DayOfWeek_id = (int)weekDay,
                        });
                    currentTimetable.Manager = timetable.Manager;
                    currentTimetable.Time = timetable[weekDay];
                    currentTimetable.Manager.Section.Cabinet = timetable.Cabinet;
                }
            }
            DatabaseContext.Entities.SaveChanges();
            DialogResult = true;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (DialogResult == true)
            {
                if (Save() == false)
                    e.Cancel = true;
                return;
            }

            MessageBoxResult result = MessageBox.Show("Хотите сохранить?", "Сохранение", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    if (Save() == false)
                        e.Cancel = true;
                    break;
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        private bool Save()
        {
            if (!(SurnameField.IsValid && NameField.IsValid && PatronymicField.IsValid))
            {
                MessageBox.Show("Неверно введены данные");
                return false;
            }

            if (Teacher == null)
                DatabaseContext.Entities.Teacher.Add(Teacher = new Teacher());

            if (Teacher.TeacherImage == null)
                Teacher.TeacherImage = new TeacherImage();

            Teacher.TeacherImage.Image_Data = Image.Source.ConvertToArray();

            Teacher.Surname = SurnameField.Text.Trim();
            Teacher.Name = NameField.Text.Trim();
            Teacher.Patronymic = PatronymicField.Text.Trim();

            DatabaseContext.Entities.SaveChanges();
            return true;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditingElement is TextBox editingCabinet && (string)e.Column.Header == "Кабинет")
            {
                editingCabinet.Text = Regex.IsMatch(editingCabinet.Text.Trim().ToLower(), @"\D") ? "" : editingCabinet.Text.Trim();
            }
            else if (e.EditingElement is TextBox editingElement)
            {
                editingElement.Text = ValidateTime(editingElement.Text);
            }
            else if (e.EditingElement is ComboBox editingCombox)
            {
                FullTimetable timetable = e.Row.Item as FullTimetable;
                if (editingCombox.SelectedItem is Section selectedSection && selectedSection != null)
                    timetable.Manager.Section = selectedSection;
            }
        }

        private string ValidateTime(string text)
        {
            Match match = Regex.Match(text.Trim(), @"^(?:(?<Datetime>\d{1,2}\:(?:\d{1,2}))|(?<Hours>\d{1,2}))");

            string dateTime = match.Groups["Datetime"].Value;
            if (match.Success)
                text = dateTime == string.Empty ? $"{match.Groups["Hours"].Value}:00" : dateTime;
            if (TimeSpan.TryParse(text, out _) == false)
                return null;
            return text;
        }

        private void OnRemoveTimetableClick(object sender, RoutedEventArgs e)
        {
            var timetables = TimetableContainer.SelectedItems.Cast<FullTimetable>();
            foreach (var fulltimetable in timetables.ToList())
            {
                foreach (var timetable in fulltimetable.Timetables)
                    timetable.IsDeleted = true;
                Timetables.Remove(fulltimetable);
            }
        }

        private void OnAddTimetableClick(object sender, RoutedEventArgs e) =>
            Timetables.Add(new FullTimetable()
            {
                Manager = new Manager()
                {
                    Teacher_id = Teacher.Id_teacher
                }
            });

        private void OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (TimetableContainer.SelectedItem != null)
            {
                (sender as DataGrid).RowEditEnding -= OnRowEditEnding;
                (sender as DataGrid).CommitEdit();
                (sender as DataGrid).Items.Refresh();
                (sender as DataGrid).RowEditEnding += OnRowEditEnding;

            }
            else return;

            SectionsView.Refresh();
            MessageBox.Show(string.Join(", ", (TimetableContainer.SelectedItem as FullTimetable).GetTimeConflicts(Timetables)));
        }

        private void OnImageClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Image Files (*.png, *.jpg, *.jpeg)|*.png|*.jpg|*.jpeg",
                DefaultExt = "Image Files (*.png, *.jpg, *.jpeg)|*.png|*.jpg|*.jpeg",
                CheckFileExists = true,
                Multiselect = false
            };

            if (fileDialog.ShowDialog() != false)
                Image.Source = File.ReadAllBytes(fileDialog.FileName).ConvertFromArray();
        }
    }
}
