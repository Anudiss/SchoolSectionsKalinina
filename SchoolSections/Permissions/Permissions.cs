using System.Collections.Generic;
using System.Linq;

namespace SchoolSections.Permissions
{
    public static class Permissions
    {
        private static readonly Dictionary<PermissionRole, Permission[]> ProhibitionPermissions = new Dictionary<PermissionRole, Permission[]>()
        {

        };

        public static bool Has(this Permission permission)
        {
            return !ProhibitionPermissions.ContainsKey(SessionData.AuthorizatedUser.PermissionRole) ||
                   !ProhibitionPermissions[SessionData.AuthorizatedUser.PermissionRole].Contains(permission);
        }
    }

    public enum PermissionRole
    {
        Director = 1,
        Teacher
    }

    public enum Permission
    {

    }
}
