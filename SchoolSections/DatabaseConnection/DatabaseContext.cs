using System.Data.Entity;

namespace SchoolSections.DatabaseConnection
{
    public static class DatabaseContext
    {
        public static SchoolEntities Entities { get; }

        static DatabaseContext()
        {
            Entities = new SchoolEntities();

            Entities.User.Load();
            Entities.Image.Load();
        }
    }
}
