using Microsoft.EntityFrameworkCore;


namespace Tests.Helpers {

    public class DataContext : DbContext {

        public static DataContext Instance => new DataContext(new DbContextOptionsBuilder()
            .UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Sifter.Tests;Trusted_Connection=True;ConnectRetryCount=0")
            .Options);

        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<SubUser> SubUsers { get; set; }
        public DbSet<Inner> Children { get; set; }

    }

}