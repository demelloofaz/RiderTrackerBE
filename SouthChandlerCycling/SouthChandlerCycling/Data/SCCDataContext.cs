using Microsoft.EntityFrameworkCore;
using SouthChandlerCycling.Models;

namespace SouthChandlerCycling.Data
{
    public class SCCDataContext : DbContext
    {
        public SCCDataContext(DbContextOptions<SCCDataContext> options) : base(options)
        {
        }

        public DbSet<Rider> Riders { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<Signup> SignUps { get; set; }
        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Rider>().ToTable("Rider");
            //modelBuilder.Entity<Ride>().ToTable("Ride");
            //modelBuilder.Entity<Signup>().ToTable("Signup");
            //modelBuilder.Entity<ICE>().ToTable("ICE");
            //modelBuilder.Entity<Bicycle>().ToTable("Bicycle");
            //modelBuilder.Entity<Buddy>().ToTable("Buddy");
            //modelBuilder.Entity<Person>().ToTable("Person");
        }
    }
}
