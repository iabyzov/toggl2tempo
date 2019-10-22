using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class SyncDbContext : DbContext
    {
        public SyncDbContext(DbContextOptions<SyncDbContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().ToTable("User");
        }
    }
}
