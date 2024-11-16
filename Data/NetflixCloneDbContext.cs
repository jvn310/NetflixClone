using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;

namespace NetflixClone.Data
{
    public class NetflixCloneDbContext : DbContext
    {
        public NetflixCloneDbContext(DbContextOptions<NetflixCloneDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Profile>().ToTable("Profiles");
        }
    }
}
