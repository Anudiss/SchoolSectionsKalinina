﻿using SchoolSections.Permissions;
using System;

namespace SchoolSections.DatabaseConnection
{
    public partial class User
    {
        public PermissionRole PermissionRole
        {
            get
            {
                try { return (PermissionRole)Role_ID; }
                catch (Exception) { throw new ArgumentException("Неизвестная роль"); }
            }
            set => Role_ID = (int)value;
        }
    }
}
