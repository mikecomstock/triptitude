﻿using System.Data.Entity;

namespace Triptitude.Biz.Models
{
    public class Db : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOptional(u => u.DefaultTrip).WithMany().Map(m => m.MapKey("DefaultTrip_Id"));
            modelBuilder.Entity<Activity>().HasMany(a => a.Tags).WithMany(t => t.Activities).Map(m => m.ToTable("ActivityTags"));

            modelBuilder.Entity<Place>().Property(p => p.Latitude).HasPrecision(9, 6);
            modelBuilder.Entity<Place>().Property(p => p.Longitude).HasPrecision(9, 6);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserTrip> UserTrips { get; set; }
        public DbSet<EmailInvite> EmailInvites { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityPlace> ActivityPlaces { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }
        public DbSet<PackingListItem> PackingListItems { get; set; }
        public DbSet<TransportationType> TransportationTypes { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
    }
}