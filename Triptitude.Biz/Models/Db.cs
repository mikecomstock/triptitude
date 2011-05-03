﻿using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Triptitude.Biz.Models
{
    public class Db : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Signup>().Property(s => s.RequestInfo).HasMaxLength(4000);

            modelBuilder.Entity<User>().HasOptional(u => u.DefaultTrip).WithMany().Map(m => m.MapKey("DefaultTrip_Id"));
            modelBuilder.Entity<User>().HasMany(u => u.Trips).WithMany(t => t.Users);

            modelBuilder.Entity<Country>().HasKey(r => r.GeoNameID).Property(p => p.GeoNameID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Region>().HasKey(r => r.GeoNameID).Property(p => p.GeoNameID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<City>().HasKey(r => r.GeoNameID).Property(p => p.GeoNameID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }

        public DbSet<Signup> Signups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Website> Websites { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
    }
}