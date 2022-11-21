namespace SchoolSections.DatabaseConnection
{
    public partial class Manager
    {
        public string TeacherSection => $"{Teacher.FullName}-{Section.Name}";
    }
}
