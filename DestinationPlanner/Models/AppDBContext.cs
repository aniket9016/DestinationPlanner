using DestinationPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace DestinationPlanner.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Country - City Relationship (One-to-Many)
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Cities)
                .WithOne(s => s.Country)
                .HasForeignKey(s => s.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Country - Destination Relationship (One-to-Many)
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Destinations)
                .WithOne(d => d.Country)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            // City - Destination Relationship (One-to-Many)
            modelBuilder.Entity<City>()
                .HasMany(c => c.Destinations)
                .WithOne(d => d.City)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Destination - Package Relationship (One-to-Many)
            modelBuilder.Entity<Destination>()
                .HasMany(d => d.Packages)
                .WithOne(p => p.Destination)
                .HasForeignKey(p => p.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Package - Booking Relationship (One-to-Many)
            modelBuilder.Entity<Package>()
                .HasMany(p => p.Bookings)
                .WithOne(b => b.Package)
                .HasForeignKey(b => b.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
