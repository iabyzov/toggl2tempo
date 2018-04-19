using BLL.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Database
{
    public class SyncDbContext : DbContext
    {
        public SyncDbContext(DbContextOptions<SyncDbContext> options) : base(options)
        {
        }

        public DbSet<WorklogEntity> Worklogs { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorklogEntity>().ToTable("Worklog");
            modelBuilder.Entity<UserEntity>().ToTable("User");
        }
    }
}
