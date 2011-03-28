using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public virtual Trip DefaultTrip { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }

    public class Trip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Created_By { get; set; }
        public DateTime Created_On { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ItineraryItem> Itinerary { get; set; }
        public virtual ICollection<Transportation> Transportations { get; set; }
        public int TotalDays
        {
            get
            {
                List<int?> days = new List<int?>
                                     {
                                         Itinerary.Max(x => x.BeginDay),
                                         Itinerary.Max(x => x.EndDay),
                                         Transportations.Max(x => x.BeginDay),
                                         Transportations.Max(x => x.EndDay) 
                                     };
                return days.Max() ?? 1;
            }
        }
        public IEnumerable<HotelPhoto> Photos
        {
            get { return Itinerary.Select(i => i.Hotel).Distinct().Where(h => h != null).SelectMany(h => h.Photos).OrderByDescending(p => p.IsDefault); }
        }

        public string StaticMapUrl
        {
            get
            {
                string url = "http://maps.google.com/maps/api/staticmap?size=500x300&maptype=roadmap&sensor=false";

                var itineraryItems = Itinerary.Where(i => i.Hotel != null).OrderBy(i => i.BeginDay).ThenBy(i => i.BeginTime);

                if (itineraryItems.Count() == 1)
                    url += "&zoom=13";
                int l = 1;
                string path = "&path=color:0x0000ff|weight:5";
                foreach (var itineraryItem in itineraryItems)
                {
                    var lat = itineraryItem.Hotel.Latitude;
                    var lon = itineraryItem.Hotel.Longitude;
                    url += string.Format("&markers=color:blue%7Clabel:{2}%7C{0},{1}", lat, lon, l++);
                    path += string.Format("|{0},{1}", lat, lon);
                }
                url += path;
                return url;
            }
        }
    }

    public class ItineraryItem
    {
        public int Id { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Website Website { get; set; }
        public virtual Hotel Hotel { get; set; }
        public int? BeginDay { get; set; }
        public TimeSpan? BeginTime { get; set; }
        public int? EndDay { get; set; }
        public TimeSpan? EndTime { get; set; }
        public virtual ICollection<Note> Notes { get; set; }

        public string Name
        {
            get
            {
                return Hotel != null ? Hotel.Name : Website != null ? Website.Title : "[No Title]";
            }
        }

        public string BeginDateTimeString
        {
            get
            {
                string timeString = null;
                if (BeginTime.HasValue)
                    timeString = DateTime.Today.Add(BeginTime.Value).ToShortTimeString();
                return string.Format("{0} Day {1}", timeString, BeginDay);
            }
        }
        public string EndDateTimeString
        {
            get
            {
                string timeString = null;
                if (EndTime.HasValue)
                    timeString = DateTime.Today.Add(EndTime.Value).ToShortTimeString();
                return string.Format("{0} Day {1}", timeString, EndDay);
            }
        }
        public string DateTimeString
        {
            get { return BeginDateTimeString + " - " + EndDateTimeString; }
        }
    }

    public class Transportation
    {
        public int Id { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual City FromCity { get; set; }
        public virtual City ToCity { get; set; }
        public int? BeginDay { get; set; }
        public int? EndDay { get; set; }
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

    #region Destinations

    // abstract!
    public abstract class Destination
    {
        public abstract int Id { get; }
        public abstract string ShortName { get; }
        public abstract string FullName { get; }
    }

    public class Country : Destination
    {
        [Key]
        public int GeoNameID { get; set; }
        public string ISO { get; set; }
        public string ISO3 { get; set; }
        public int ISONumeric { get; set; }
        public string FIPS { get; set; }

        public override int Id
        {
            get { return GeoNameID; }
        }

        public override string FullName
        {
            get { return Name; }
        }

        public override string ShortName
        {
            get { return Name; }
        }

        public string Name { get; set; }
        public string Capital { get; set; }
        public int Population { get; set; }
        public string Continent { get; set; }
        public string TLD { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string Phone { get; set; }
        public string PostalCodeFormat { get; set; }
        public string PostalCodeRegex { get; set; }
        public string Languages { get; set; }
        public string Neighbours { get; set; }
        public string EquivalentFipsCode { get; set; }
    }

    public class Region : Destination
    {
        [Key]
        public int GeoNameID { get; set; }
        public string ASCIIName { get; set; }
        public virtual Country Country { get; set; }
        public string GeoNameAdmin1Code { get; set; }

        public override int Id
        {
            get { return GeoNameID; }
        }

        public override string FullName
        {
            get { return ASCIIName + ", " + Country.FullName; }
        }

        public override string ShortName
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

        public override int Id
        {
            get { return GeoNameID; }
        }

        public override string FullName
        {
            get { return ASCIIName + ", " + Region.FullName; }
        }

        public override string ShortName
        {
            get { return ASCIIName; }
        }
    }

    #endregion

    [Table("HotelPhotos")]
    public class HotelPhoto
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string ThumbURL { get; set; }
        public bool IsDefault { get; set; }
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
        public bool? HasContinentalBreakfast { get; set; }
        public virtual ICollection<HotelPhoto> Photos { get; set; }
        public virtual ICollection<ItineraryItem> ItineraryItems { get; set; }

        public IEnumerable<Trip> Trips { get { return ItineraryItems.Select(ii => ii.Trip).Distinct(); } }
        public HotelPhoto DefaultPhoto
        {
            get
            {
                var photo = Photos.FirstOrDefault(p => p.IsDefault);
                return photo;
            }
        }
    }
}