using SchoolSections.DatabaseConnection;
using SchoolSections.Permissions;
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

namespace SchoolSections.Windows.AuthWindow
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        #region Error message
        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(AuthWindow));
        #endregion

        public AuthWindow() => InitializeComponent();

        private void OnLogIn(object sender, RoutedEventArgs e)
        {
            try
            {
                SessionData.AuthorizatedUser = TryAuthorizeUser();
                new MainWindow.MainWindow().Show();
                Close();
            }
            catch (ArgumentException exception) { ErrorMessage = exception.Message; }
        }

        private User TryAuthorizeUser()
        {
            string login = LoginComponent.Text;
            string password = PasswordComponent.Password;

            User authorizedUser = DatabaseContext.Entities.User.FirstOrDefault(user => user.Login == login &&
                                                                                       user.Password == password);
            if (authorizedUser == default)
                throw new ArgumentException("Неверный логин или пароль");

            return authorizedUser;
        }
    }
}
