﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Triptitude.Biz.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string AnonymousId { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? GuidCreatedOnUtc { get; set; }

        public virtual Trip DefaultTrip { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }

        public bool GuidIsExpired
        {
            get
            {
                if (!Guid.HasValue || !GuidCreatedOnUtc.HasValue) return true;
                DateTime guidExpiresOn = GuidCreatedOnUtc.Value.AddMonths(1);
                return guidExpiresOn < DateTime.UtcNow;
            }
        }

        public string LoginLinkUrl
        {
            get
            {
                string root = ConfigurationManager.AppSettings["RootUrl"];
                string path = Path.Combine(root, "login?token=" + Guid.Value);
                return path;
            }
        }
    }

    public class Trip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual User User { get; set; }
        public DateTime Created_On { get; set; }
        public DateTime? BeginDate { get; set; }
        public bool ShowInSiteMap { get; set; }
        public bool Moderated { get; set; }
        public bool ShowInSite { get; set; }

        public IEnumerable<User> Users { get { return new[] { User }; } }
        public virtual ICollection<Activity> Activities { get; set; }

        public int TotalDays
        {
            get
            {
                return Activities.Any() ? Activities.Max(a => a.EndDay) : 1;
            }
        }

        public IEnumerable<HotelPhoto> Photos
        {
            get { return Activities.OfType<HotelActivity>().Select(ha => ha.Hotel).Select(h => h.Photo); }
        }

        public IEnumerable<City> Cities
        {
            get
            {
                IEnumerable<City> cities = Activities.SelectMany(a => a.Cities).ToList();
                IEnumerable<City> grouped = from c in cities
                                            group c by c into grp
                                            let count = grp.Count()
                                            orderby count
                                            select grp.Key;
                return grouped;
            }
        }
    }

    #region Activities

    [Table("Activities")]
    public class Activity
    {
        public int Id { get; set; }
        public virtual Trip Trip { get; set; }
        public int BeginDay { get; set; }
        public TimeSpan? BeginTime { get; set; }
        public int EndDay { get; set; }
        public TimeSpan? EndTime { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        // From CityActivities view
        public virtual ICollection<City> Cities { get; set; }
    }

    [Table("TransportationActivities")]
    public class TransportationActivity : Activity
    {
        public virtual TransportationType TransportationType { get; set; }
        public virtual City FromCity { get; set; }
        public virtual City ToCity { get; set; }
    }

    [Table("HotelActivities")]
    public class HotelActivity : Activity
    {
        public virtual Hotel Hotel { get; set; }
    }

    [Table("WebsiteActivities")]
    public class WebsiteActivity : Activity
    {
        public string URL { get; set; }
        public string Title { get; set; }

        public enum ThumbSize { Small, Medium, Large }

        public static string ThumbFilename(int websiteId, ThumbSize thumbSize)
        {
            switch (thumbSize)
            {
                case ThumbSize.Small: return websiteId + "-small.jpg";
                case ThumbSize.Medium: return websiteId + "-medium.jpg";
                case ThumbSize.Large: return websiteId + "-large.jpg";
                default: throw new Exception();
            }
        }
    }

    [Table("PlaceActivities")]
    public class PlaceActivity : Activity
    {
        public virtual Place Place { get; set; }
    }

    #endregion

    public class TransportationType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PathType
        {
            get
            {
                switch (Name)
                {
                    case "Fly": return "geodesic";
                    case "Train": return "geodesic";
                    default: return "directions";
                }
            }
        }
    }

    public class Note
    {
        public int Id { get; set; }
        public virtual Activity Activity { get; set; }
        public int Created_By { get; set; }
        public DateTime Created_On { get; set; }
        public string Text { get; set; }
        public bool Public { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }

        // Needed to make Html.ListBoxFor work in the admin section.
        public override string ToString() { return Id.ToString(); }

        public string NiceName
        {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Name); }
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LinkURL { get; set; }
        public string ImageURL { get; set; }
        public string ASIN { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }

    #region Destinations

    // abstract!
    public interface IDestination
    {
        int GeoNameID { get; set; }
        string ShortName { get; }
        string FullName { get; }
        IQueryable<Tag> Tags { get; }
    }

    public class Country : IDestination
    {
        [Key]
        public int GeoNameID { get; set; }
        public string ISO { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Region> Regions { get; set; }

        public string FullName
        {
            get { return Name; }
        }

        public IQueryable<Tag> Tags
        {
            get { return Regions.SelectMany(r => r.Tags).AsQueryable(); }
        }

        public string ShortName
        {
            get { return Name; }
        }

    }

    public class Region : IDestination
    {
        [Key]
        public int GeoNameID { get; set; }
        public string ASCIIName { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public string GeoNameAdmin1Code { get; set; }

        public string FullName
        {
            get { return ASCIIName + ", " + Country.FullName; }
        }

        public IQueryable<Tag> Tags
        {
            get { return Cities.SelectMany(c => c.Tags).AsQueryable(); }
        }

        public string ShortName
        {
            get { return ASCIIName; }
        }
    }

    public class City : IDestination
    {
        [Key]
        public int GeoNameID { get; set; }
        public string ASCIIName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public virtual Region Region { get; set; }
        public virtual IQueryable<Tag> Tags { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }

        public string FullName
        {
            get { return ASCIIName + ", " + Region.FullName; }
        }

        public string ShortName
        {
            get { return ASCIIName; }
        }

        public IEnumerable<Tag> TagsToShow
        {
            get
            {
                var placeActivities = Activities.OfType<PlaceActivity>().Where(ta => ta.Trip.ShowInSite);
                IEnumerable<Tag> tags = placeActivities.Select(pa => pa.Tags.First()).GroupBy(ta => ta).OrderByDescending(g => g.Count()).Select(g => g.Key);
                return tags;
            }
        }
    }

    #endregion

    #region Hotels

    public class HotelPhoto
    {
        public string ImageURL { get; set; }
        public string ThumbURL { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int NiceHeight { get { return 250; } }
    }

    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int Image_Id { get; set; }
        public int NumberOfReviews { get; set; }
        public decimal ConsumerRating { get; set; }
        public virtual ICollection<HotelActivity> HotelActivities { get; set; }

        public IEnumerable<Trip> Trips { get { return HotelActivities.Select(a => a.Trip).Where(t => t.ShowInSite).Distinct(); } }

        public HotelPhoto Photo
        {
            get
            {
                var photo = new HotelPhoto
                        {
                            ImageURL = string.Format("http://media.hotelscombined.com/HI{0}.jpg", Image_Id),
                            ThumbURL = string.Format("http://media.hotelscombined.com/HT{0}.jpg", Image_Id)
                        };
                return photo;
            }
        }
    }

    #endregion

    public class Place
    {
        public int Id { get; set; }
        public string FactualId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AddressExtended { get; set; }
        public string POBox { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Category { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Status { get; set; }

        public string NiceAddress
        {
            get
            {
                var parts = new[] { Address, Locality, Region, Country };
                var full = String.Join(", ", parts);
                return full;
            }
        }

        public static Place FromEntityJson(dynamic json)
        {
            Place p = new Place
            {
                Address = json.address,
                AddressExtended = json.address_extended,
                Category = json.category == null ? null : ((string)json.category).Trim('"'),
                Country = json.country,
                Email = json.email,
                FactualId = json.factual_id,
                Fax = json.fax,
                Latitude = json.latitude,
                Locality = json.locality,
                Longitude = json.longitude,
                Name = json.name,
                POBox = json.po_box,
                PostCode = json.postcode,
                Region = json.region,
                Status = json.status,
                Telephone = json.tel,
                Website = json.website
            };
            return p;
        }

        public static Place FromSearchJson(dynamic json)
        {
            //"subject_key","factual_id","name","address","address_extended","po_box","locality","region","country","postcode","tel","fax","category","website","email","latitude","longitude","status"
            Place p = new Place
            {
                FactualId = json[1],
                Name = json[2],
                Address = json[3],
                AddressExtended = json[4],
                POBox = json[5],
                Locality = json[6],
                Region = json[7],
                Country = json[8],
                PostCode = json[9],
                Telephone = json[10],
                Fax = json[11],
                Category = json[12] == null ? null : ((string)json[12]).Trim('"'),
                Website = json[13],
                Email = json[14],
                Latitude = json[15],
                Longitude = json[16],
                Status = json[17]
            };
            return p;
        }
    }
}