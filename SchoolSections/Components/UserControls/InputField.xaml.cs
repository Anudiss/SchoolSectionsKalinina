using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SchoolSections.Components.UserControls
{
    /// <summary>
    /// Обработчик события валидации
    /// </summary>
    /// <param name="sender">Инициатор события</param>
    /// <param name="args">Аргументы события</param>
    public delegate bool ValidateEventHandler(object sender, ValidateEventArgs args);
    
    /// <summary>
    /// Логика взаимодействия для InputField.xaml
    /// </summary>
    public partial class InputField : UserControl
    {
        #region Text properties
        #region TextChanged event
        /// <summary>
        /// Событие изменения текста
        /// </summary>
        public event TextChangedEventHandler TextChanged
        {
            add => TextComponent.TextChanged += value;
            remove => TextComponent.TextChanged -= value;
        }
        #endregion
        #region Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InputField));
        #endregion
        #endregion
        #region Label properties
        #region Label
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(InputField));
        #endregion
        #region Label font size
        public int LabelFontSize
        {
            get { return (int)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(int), typeof(InputField), new PropertyMetadata(14));
        #endregion
        #region Label font weight
        public FontWeight LabelFontWeight
        {
            get { return (FontWeight)GetValue(LabelFontWeightProperty); }
            set { SetValue(LabelFontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelFontWeightProperty =
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(InputField), new PropertyMetadata(FontWeights.Medium));
        #endregion
        #region Label foreground
        public SolidColorBrush LabelForeground
        {
            get { return (SolidColorBrush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(SolidColorBrush), typeof(InputField), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        #endregion
        #endregion
        #region Is valid
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsValid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(InputField));
        #endregion
        #region Is validable
        public bool IsValidable
        {
            get { return (bool)GetValue(IsValidableProperty); }
            set { SetValue(IsValidableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsValidable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsValidableProperty =
            DependencyProperty.Register("IsValidable", typeof(bool), typeof(InputField), new PropertyMetadata(false));
        #endregion

        public event ValidateEventHandler OnValidate;

        public InputField() => InitializeComponent();

        private void OnTextChanged(object sender, TextChangedEventArgs e) =>
            IsValid = OnValidate?.Invoke(this, new ValidateEventArgs() { Text = Text ?? string.Empty }) == true || !IsValidable;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IsValid = OnValidate?.Invoke(this, new ValidateEventArgs() { Text = Text ?? string.Empty }) == true || !IsValidable;
        }
    }

    public struct ValidateEventArgs
    {
        public string Text { get; set; }
    }

    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class ValidationColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isValid == false)
                return new SolidColorBrush(Colors.LightBlue);

            return isValid ? new SolidColorBrush(Colors.LightBlue) : Application.Current.FindResource("IncorrectInputSolidBrush");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
