using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Triptitude.Biz.Models
{
    public class Db : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOptional(u => u.DefaultTrip).WithMany().Map(m => m.MapKey("DefaultTrip_Id"));

            modelBuilder.Entity<Activity>().HasMany(a => a.Tags).WithMany(t => t.Activities).Map(m => m.ToTable("ActivityTags"));

            modelBuilder.Entity<Country>().HasKey(r => r.GeoNameID).Property(p => p.GeoNameID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Region>().HasKey(r => r.GeoNameID).Property(p => p.GeoNameID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<City>().HasKey(r => r.GeoNameID).Property(p => p.GeoNameID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Place>().Property(p => p.Latitude).HasPrecision(9, 6);
            modelBuilder.Entity<Place>().Property(p => p.Longitude).HasPrecision(9, 6);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Trip> Trips { get; set; }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<TransportationActivity> TransportationActivities { get; set; }
        public DbSet<HotelActivity> HotelActivities { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Item> Items { get; set; }

        public DbSet<TransportationType> TransportationTypes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
    }
}