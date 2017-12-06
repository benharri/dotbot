using Microsoft.EntityFrameworkCore;

namespace dotbot.Core
{
    public class DotbotDbContext : DbContext
    {
        public DbSet<Definition> Defs { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<UserLocation> UserLocations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=dotbot.db");
        }
    }

}
