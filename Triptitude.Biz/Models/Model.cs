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

        public IEnumerable<BaseItemPhoto> Photos
        {
            get { return Itinerary.Select(i => i.BaseItem).Distinct().Where(bi => bi != null).SelectMany(bi => bi.Photos).OrderByDescending(p => p.IsDefault); }
        }
    }

    public class ItineraryItem
    {
        public int Id { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Website Website { get; set; }
        public virtual BaseItem BaseItem { get; set; }
        public int? BeginDay { get; set; }
        public TimeSpan? BeginTime { get; set; }
        public int? EndDay { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool SoftDeleted { get; set; }

        public string Name
        {
            get
            {
                return
                    BaseItem != null ? BaseItem.Name :
                    Website != null ? Website.Title : "[No Title]";
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

        public ExpediaHotel Hotel
        {
            get { return new ExpediaHotelsRepo().FindByBaseItemId(BaseItem.Id); }
        }
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

    public class Country
    {
        [Key]
        public int GeoNameID { get; set; }
        public string ISO { get; set; }
        public string ISO3 { get; set; }
        public int ISONumeric { get; set; }
        public string FIPS { get; set; }
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

    public class Region
    {
        [Key]
        public int GeoNameID { get; set; }
        public string ASCIIName { get; set; }
        public virtual Country Country { get; set; }
        public string GeoNameAdmin1Code { get; set; }
    }

    public class City
    {
        [Key]
        public int GeoNameID { get; set; }
        public string ASCIIName { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public virtual Region Region { get; set; }
    }

    public class BaseItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ItemType { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public virtual ICollection<BaseItemPhoto> Photos { get; set; }
        public virtual ICollection<ItineraryItem> ItineraryItems { get; set; }

        public IEnumerable<Trip> Trips { get { return ItineraryItems.Select(ii => ii.Trip).Distinct(); } }
    }

    [Table("BaseItemPhotos")]
    public class BaseItemPhoto
    {
        public int Id { get; set; }
        public int BaseItemId { get; set; }
        public string ImageURL { get; set; }
        public string ThumbURL { get; set; }
        public bool IsDefault { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public int NiceHeight { get { return 250; } }
    }

    public class ExpediaHotel
    {
        [Key]
        public int ExpediaHotelId { get; set; }
        public virtual BaseItem BaseItem { get; set; }
        public bool? HasContinentalBreakfast { get; set; }
    }
}