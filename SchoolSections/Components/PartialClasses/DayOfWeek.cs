namespace SchoolSections.DatabaseConnection
{
    public partial class DayOfWeek
    {
        public System.DayOfWeek WeekDay
        {
            get => (System.DayOfWeek)Id_dayOfWeek;
            set => Id_dayOfWeek = (int)value;
        }
    }
}
