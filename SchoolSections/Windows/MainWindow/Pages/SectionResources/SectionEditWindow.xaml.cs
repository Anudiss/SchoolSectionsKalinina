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
using System.Windows.Navigation;
using Section = SchoolSections.DatabaseConnection.Section;

namespace SchoolSections.Windows.MainWindow.Pages.SectionResources
{
    /// <summary>
    /// Логика взаимодействия для SectionEditWindow.xaml
    /// </summary>
    public partial class SectionEditWindow : Window
    {
        #region Section
        public Section Section
        {
            get { return (Section)GetValue(SectionProperty); }
            set
            {
                SetValue(SectionProperty, value);
                CapacityField.IsErrorOpen = value == null;
                if (value == null)
                    Image.Source = DatabaseConnection.Image.GetImageByName("noimage").BitmapImage;
                else
                {
                    Timetables = new ObservableCollection<FullTimetable>(value.Timetables);
                }
            }
        }

        // Using a DependencyProperty as the backing store for Section.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionProperty =
            DependencyProperty.Register("Section", typeof(Section), typeof(SectionEditWindow));
        #endregion
        #region Teachers
        public ObservableCollection<Teacher> Teachers
        {
            get { return (ObservableCollection<Teacher>)GetValue(TeachersProperty); }
            set { SetValue(TeachersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Teachers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TeachersProperty =
            DependencyProperty.Register("Teachers", typeof(ObservableCollection<Teacher>), typeof(SectionEditWindow));
        #endregion
        #region Timetables
        public ObservableCollection<FullTimetable> Timetables
        {
            get { return (ObservableCollection<FullTimetable>)GetValue(TimetablesProperty); }
            set { SetValue(TimetablesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Timetables.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimetablesProperty =
            DependencyProperty.Register("Timetables", typeof(ObservableCollection<FullTimetable>), typeof(SectionEditWindow));
        #endregion

        public SectionEditWindow()
        {
            InitializeComponent();

            Teachers = DatabaseContext.Entities.Teacher.Local;
        }


        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditingElement is TextBox editingElement)
            {
                editingElement.Text = ValidateTime(editingElement.Text);
            }
            else if (e.EditingElement is ComboBox editingCombox)
            {
                FullTimetable timetable = e.Row.Item as FullTimetable;
                timetable.Manager.Teacher = editingCombox.SelectedItem as Teacher;
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

        private bool ValidateTime(string text, out string time)
        {
            return (time = ValidateTime(text)) != null;
        }

        public bool OnCapacityValidate(object sender, Components.UserControls.ValidateEventArgs args) =>
            int.TryParse(args.Text.Trim(), out int capacity) && capacity > 0 && capacity <= 25;

        private void OnSave(object sender, RoutedEventArgs e)
        {
            OnSaveClick();
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

        private void OnSaveClick()
        {
            if (Timetables.Any(timetable => timetable.Manager.Teacher == null))
            {
                MessageBox.Show("Некоторые преподаватели не заполнены");
                return;
            }

            if (Timetables.Count() != Timetables.Select(timetable => timetable.Manager.Teacher).Distinct().Count())
            {
                MessageBox.Show("Кружок не можут вести разные учителя");
                return;
            }

            ObservableCollection<Timetable> managerTimetables = DatabaseContext.Entities.Timetable.Local;
            foreach (var timetable in Timetables)
            {
                IEnumerable<Timetable> tables = managerTimetables.Where(table => table.Manager == timetable.Manager);
                foreach (var weekDay in Enum.GetValues(typeof(System.DayOfWeek)).Cast<System.DayOfWeek>())
                {
                    Timetable currentTimetable = tables.FirstOrDefault(table => table.DayOfWeek_id == (int)weekDay);
                    if (currentTimetable == null)
                        managerTimetables.Add(currentTimetable = new Timetable()
                        {
                            DayOfWeek_id = (int)weekDay,
                        });
                    currentTimetable.Manager = timetable.Manager;
                    currentTimetable.Time = timetable[weekDay];
                }
            }
            DatabaseContext.Entities.SaveChanges();
            DialogResult = true;
        }

        private bool Save()
        {
            if (!(TitleField.IsValid && CapacityField.IsValid && DurationField.IsValid))
            {
                MessageBox.Show("Неверно введены данные");
                return false;
            }

            if (Section == null)
                DatabaseContext.Entities.Section.Add(Section = new Section());

            if (Section.SectionImage == null)
                Section.SectionImage = new SectionImage();

            Section.SectionImage.Image_Data = Image.Source.ConvertToArray();

            Section.Name = TitleField.Text;
            Section.Capacity = int.Parse(CapacityField.Text);
            Section.Duration = TimeSpan.Parse(DurationField.Text);

            DatabaseContext.Entities.SaveChanges();
            return true;
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

        public void OnManagerDelete(Manager manager)
        {
            manager.IsDeleted = true;
            DatabaseContext.Entities.SaveChanges();
        }

        private void OnRemoveTimetableClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnAddTimetableClick(object sender, RoutedEventArgs e)
        {
            Timetables.Add(new FullTimetable()
            {
                Manager = new Manager()
                {
                    Section_id = Section.Id_section
                }
            });
        }

        public bool OnDurationValidate(object sender, Components.UserControls.ValidateEventArgs args)
        {
            return ValidateTime(DurationField.Text.Trim(), out string _);
        }

        public void OnDurationFieldTextChangingEnd()
        {
            string time = ValidateTime(DurationField.Text);
            DurationField.Text = TimeSpan.Parse(time ?? DurationField.Text).ToString(@"hh\:mm");
        }
    }
}
