//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SchoolSections.DatabaseConnection
{
    using System;
    using System.Collections.Generic;
    
    public partial class Attendance_students
    {
        public int Id_attendance_students { get; set; }
        public int Attendance_id { get; set; }
        public int Student_id { get; set; }
    
        public virtual Attendance Attendance { get; set; }
        public virtual Student Student { get; set; }
    }
}
