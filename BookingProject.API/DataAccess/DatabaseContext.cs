using BookingProject.API.Entities;
using Microsoft.EntityFrameworkCore;
namespace BookingProject.API.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOne<Bookings>().WithOne(x=> x.user).HasForeignKey<Bookings>(x=> x.user_id);
            modelBuilder.Entity<Apartment>().HasOne<Bookings>().WithOne(s => s.apartment).HasForeignKey<Bookings>(x=> x.apartment_id);

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Apartment> Appartments { get; set; }


    }
}
