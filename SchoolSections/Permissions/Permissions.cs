using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SchoolSections.Permissions
{
    public static class Permissions
    {
        private static readonly Dictionary<PermissionRole, Permission[]> ProhibitionPermissions = new Dictionary<PermissionRole, Permission[]>()
        {
            
        };

        public static bool Has(this Permission permission)
        {
            return !(ProhibitionPermissions.ContainsKey(SessionData.AuthorizatedUser.PermissionRole) &&
                     ProhibitionPermissions[SessionData.AuthorizatedUser.PermissionRole].Contains(permission));
        }
    }

    [ValueConversion(typeof(PermissionRole), typeof(Visibility))]
    public class PermissionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Permission permission == false)
                return default(Visibility);

            return permission.Has() ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public enum PermissionRole
    {
        Director = 1,
        Teacher
    }

    public enum Permission
    {
        Add
    }
}
