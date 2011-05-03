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
        public DateTime? BeginDate { get; set; }
        public int Created_By { get; set; }
        public DateTime Created_On { get; set; }
        public bool ShowInSiteMap { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ItineraryItem> Itinerary { get; set; }
        public virtual ICollection<Transportation> Transportations { get; set; }
        public int TotalDays
        {
            get
            {
                List<int?> days = new List<int?>
                                     {
                                         Itinerary.Any() ? Itinerary.Max(x => x.BeginDay) : 0,
                                         Itinerary.Any() ? Itinerary.Max(x => x.EndDay) : 0,
                                         Transportations.Any() ? Transportations.Max(x => x.BeginDay) : 0,
                                         Transportations.Any() ? Transportations.Max(x => x.EndDay) : 0
                                     };
                return Math.Max(days.Max().Value, 1);
            }
        }
        public IEnumerable<HotelPhoto> Photos
        {
            get { return Itinerary.Select(i => i.Hotel).Distinct().Where(h => h != null).Select(h => h.Photo); }
        }
    }

    public class ItineraryItem
    {
        public int Id { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Website Website { get; set; }
        public virtual Hotel Hotel { get; set; }
        public virtual DestinationTag DestinationTag { get; set; }
        public int BeginDay { get; set; }
        public TimeSpan? BeginTime { get; set; }
        public int EndDay { get; set; }
        public TimeSpan? EndTime { get; set; }
        public virtual ICollection<Note> Notes { get; set; }

        public string Name
        {
            get
            {
                return Hotel != null ? Hotel.Name : Website != null ? Website.Title : DestinationTag != null ? DestinationTag.Tag.Name : "[No Title]";
            }
        }
    }

    public class Transportation
    {
        public int Id { get; set; }
        public virtual TransportationType TransportationType { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual City FromCity { get; set; }
        public virtual City ToCity { get; set; }
        public int BeginDay { get; set; }
        public int EndDay { get; set; }
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
        public virtual ItineraryItem ItineraryItem { get; set; }
        public DateTime Created_On { get; set; }
        public int Created_By { get; set; }
        public string Text { get; set; }
        public bool Public { get; set; }
    }

    public class Website
    {
        public int Id { get; set; }
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

        public virtual ICollection<ItineraryItem> ItineraryItems { get; set; }

        public IEnumerable<Trip> Trips
        {
            get { return ItineraryItems.Select(ii => ii.Trip); }
        }
    }

    public class DestinationTag
    {
        public int Id { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual City City { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    #region Destinations

    // abstract!
    public interface Destination
    {
        int GeoNameID { get; set; }
        string ShortName { get; }
        string FullName { get; }
    }

    public class Country : Destination
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

        public string ShortName
        {
            get { return Name; }
        }

    }

    public class Region : Destination
    {
        [Key]
        public int GeoNameID { get; set; }
        public string ASCIIName { get; set; }
        public virtual Country Country { get; set; }
        public string GeoNameAdmin1Code { get; set; }

        public string FullName
        {
            get { return ASCIIName + ", " + Country.FullName; }
        }

        public string ShortName
        {
            get { return ASCIIName; }
        }
    }

    public class City : Destination
    {
        [Key]
        public int GeoNameID { get; set; }
        public string ASCIIName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public virtual Region Region { get; set; }

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
        public virtual ICollection<ItineraryItem> ItineraryItems { get; set; }

        public IEnumerable<Trip> Trips { get { return ItineraryItems.Select(ii => ii.Trip).Distinct(); } }

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