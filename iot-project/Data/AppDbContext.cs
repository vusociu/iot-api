using iot_project.Models;
using Microsoft.EntityFrameworkCore;
namespace iot_project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<IdentityCard> IdentityCards { get; set; }
        public DbSet<CheckCardHistory> CheckCardHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => { entity.HasIndex(e => e.email).IsUnique(); });
            modelBuilder.Entity<IdentityCard>(entity => { entity.HasIndex(e => e.idCard).IsUnique(); });
        }
    }
}
