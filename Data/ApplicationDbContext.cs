using Microsoft.EntityFrameworkCore;
using TravelBookingSystem.Models;

namespace TravelBookingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public  ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Destinations>? Destinations { get; set; }
        public DbSet<Bookings>? Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bookings>()
                .HasKey(b => b.BookingId); // Explicitly set BookingId as primary key

            modelBuilder.Entity<Destinations>()
                .HasKey(d => d.DestinationId); // Primary key for Destinations

            base.OnModelCreating(modelBuilder);
        }
    }
}
