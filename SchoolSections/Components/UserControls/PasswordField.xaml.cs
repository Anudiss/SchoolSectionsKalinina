using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SchoolSections.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для InputField.xaml
    /// </summary>
    public partial class PasswordField : UserControl
    {
        #region PasswordBox properties
        #region PasswordChanged event
        /// <summary>
        /// Событие изменения текста
        /// </summary>
        public event RoutedEventHandler PasswordChanged
        {
            add => PasswordComponent.PasswordChanged += value;
            remove => PasswordComponent.PasswordChanged -= value;
        }
        #endregion
        #region Password
        public string Password => PasswordComponent.Password;
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
            DependencyProperty.Register("Label", typeof(string), typeof(PasswordField));
        #endregion
        #region Label font size
        public int LabelFontSize
        {
            get { return (int)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(int), typeof(PasswordField), new PropertyMetadata(14));
        #endregion
        #region Label font weight
        public FontWeight LabelFontWeight
        {
            get { return (FontWeight)GetValue(LabelFontWeightProperty); }
            set { SetValue(LabelFontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelFontWeightProperty =
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(PasswordField), new PropertyMetadata(FontWeights.Medium));
        #endregion
        #region Label foreground
        public SolidColorBrush LabelForeground
        {
            get { return (SolidColorBrush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(SolidColorBrush), typeof(PasswordField), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
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
            DependencyProperty.Register("IsValid", typeof(bool), typeof(PasswordField));
        #endregion
        #region Is validable
        public bool IsValidable
        {
            get { return (bool)GetValue(IsValidableProperty); }
            set { SetValue(IsValidableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsValidable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsValidableProperty =
            DependencyProperty.Register("IsValidable", typeof(bool), typeof(PasswordField), new PropertyMetadata(false));
        #endregion

        public event ValidateEventHandler OnValidate;

        public PasswordField() => InitializeComponent();

        private void OnPasswordChanged(object sender, RoutedEventArgs e) =>
            IsValid = OnValidate?.Invoke(this, new ValidateEventArgs() { Text = Password ?? string.Empty }) == true || !IsValidable;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IsValid = OnValidate?.Invoke(this, new ValidateEventArgs() { Text = Password ?? string.Empty }) == true || !IsValidable;
        }
    }
}
