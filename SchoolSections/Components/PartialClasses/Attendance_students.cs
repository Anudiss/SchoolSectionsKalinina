namespace SchoolSections.DatabaseConnection
{
    public partial class Attendance_students
    {
        public void Delete()
        {
            if (IsDeleted == true)
                return;

            IsDeleted = true;
        }
    }
}
