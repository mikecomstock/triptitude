using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Triptitude.Biz.Models
{
    public class Db : DbContext
    {
        protected override void OnModelCreating(System.Data.Entity.ModelConfiguration.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOptional(u => u.DefaultTrip).WithMany().IsIndependent().Map(m => m.MapKey(u => u.Id, "DefaultTripId"));
            modelBuilder.Entity<User>().HasMany(u => u.Trips).WithMany(t => t.Users);

            modelBuilder.Entity<Country>().HasKey(r => r.GeoNameID).Property(p => p.GeoNameID).HasDatabaseGenerationOption(DatabaseGenerationOption.None);
            modelBuilder.Entity<Region>().HasKey(r => r.GeoNameID).Property(p => p.GeoNameID).HasDatabaseGenerationOption(DatabaseGenerationOption.None);
            modelBuilder.Entity<City>().HasKey(r => r.GeoNameID).Property(p => p.GeoNameID).HasDatabaseGenerationOption(DatabaseGenerationOption.None);

            modelBuilder.Entity<ExpediaHotel>().HasKey(r => r.ExpediaHotelId).Property(p => p.ExpediaHotelId).HasDatabaseGenerationOption(DatabaseGenerationOption.None);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Website> Websites { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<BaseItem> BaseItems { get; set; }
        public DbSet<ExpediaHotel> ExpediaHotels { get; set; }
    }
}