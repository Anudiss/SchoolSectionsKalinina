namespace SchoolSections.DatabaseConnection
{
    public partial class Student
    {
        public string FullName => $"{Surname} {Name} {Partonymic}";
    }
}
