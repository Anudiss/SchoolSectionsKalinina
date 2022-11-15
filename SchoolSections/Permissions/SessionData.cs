using SchoolSections.DatabaseConnection;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SchoolSections.Permissions
{
    public class SessionData
    {
        public static User AuthorizatedUser
        {
            get;
            set;
        }
    }
}
