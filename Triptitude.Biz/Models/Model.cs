using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Triptitude.Biz.Models
{
    public class Signup
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string TripName { get; set; }
        public int GeoNameId { get; set; }
        public string IP { get; set; }
        public string RequestInfo { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }

        public virtual Trip DefaultTrip { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }

    public class Trip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Created_By { get; set; }
        public DateTime Created_On { get; set; }
        public DateTime? BeginDate { get; set; }
        public bool ShowInSiteMap { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }

        public int TotalDays
        {
            get
            {
                return Activities.Any() ? Activities.Max(a => a.EndDay) : 1;
            }
        }
    }

    [Table("Activities")]
    public class Activity
    {
        public int Id { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual City City { get; set; }
        public int BeginDay { get; set; }
        public TimeSpan? BeginTime { get; set; }
        public int EndDay { get; set; }
        public TimeSpan? EndTime { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        [NotMapped]
        public virtual LodgingActivity LodgingActivity { get; set; }
        [NotMapped]
        public virtual WebsiteActivity WebsiteActivity { get; set; }
        //public virtual ICollection<TransportationActivity> TransportationActivities { get; set; }
        //public virtual TransportationActivity TransportationActivity { get { return TransportationActivities.FirstOrDefault(); } }

        public string Name
        {
            get
            {
                return "activity name here";// return Hotel != null ? Hotel.Name : Website != null ? Website.Title : DestinationTag != null ? DestinationTag.Tag.Name : "[No Title]";
            }
        }
    }

    [Table("TransportationActivities")]
    public class TransportationActivity : Activity
    {
        public virtual TransportationType TransportationType { get; set; }
        public virtual City FromCity { get; set; }
        public virtual City ToCity { get; set; }
    }

    public class TransportationType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TravelMode
        {
            get
            {
                switch (Name)
                {
                    case "Fly":
                    case "Boat": return "line";
                    default: return "road";
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

    public class WebsiteActivity
    {
        public int Id { get; set; }
        public virtual Activity Activity { get; set; }
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

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
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

        public string FullName
        {
            get { return ASCIIName + ", " + Region.FullName; }
        }

        public string ShortName
        {
            get { return ASCIIName; }
        }
    }

    #endregion

    #region Hotels

    public class LodgingActivity
    {
        public int Id { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual Hotel Hotel { get; set; }
    }

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
        public virtual ICollection<LodgingActivity> LodgingActivities { get; set; }

        public IEnumerable<Trip> Trips { get { return LodgingActivities.Select(la => la.Activity).Select(a => a.Trip).Distinct(); } }

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
}