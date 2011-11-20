using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
        public virtual ICollection<Note> Notes { get; set; }

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

        public bool OwnsTrips(params Trip[] trips)
        {
            var userOwnsAllTrips = trips.All(t => t.User == this);
            return userOwnsAllTrips;
        }
    }

    public class Trip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual User User { get; set; }
        public DateTime Created_On { get; set; }
        public DateTime? BeginDate { get; set; }
        public bool ShowInSearch { get; set; }
        public DateTime? ModeratedOnUTC { get; set; }

        public IEnumerable<User> Users { get { return new[] { User }; } }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<PackingListItem> PackingListItems { get; set; }

        public int TotalDays
        {
            get
            {
                int? max = Activities.Select(a => a.BeginDay).Union(Activities.Select(a => a.EndDay)).Max();
                return max ?? 1;
            }
        }

        public IEnumerable<HotelPhoto> Photos
        {
            get { return Activities.OfType<HotelActivity>().Select(ha => ha.Hotel).Select(h => h.Photo); }
        }

        public IEnumerable<Activity> ActivitiesOn(int? dayNumber)
        {
            return dayNumber.HasValue
                ? Activities.Where(a => a.BeginDay == dayNumber || a.EndDay == dayNumber)
                : Activities.Where(a => !a.BeginDay.HasValue && !a.EndDay.HasValue);
        }

        public IEnumerable<Tag> Tags
        {
            get
            {
                var tags = Activities.SelectMany(a => a.Tags).Distinct();
                return tags;
            }
        }

        public string NiceDay(int? dayNumer, User currentUser)
        {
            if (!dayNumer.HasValue)
            {
                return "Unscheduled Activities";
            }
            else if (BeginDate.HasValue && (BeginDate.Value.AddDays(TotalDays - 1) < DateTime.Today || currentUser.OwnsTrips(this)))
            {
                return string.Format("Day {0} - {1:dddd, MMM dd}", dayNumer, BeginDate.Value.AddDays(dayNumer.Value - 1));
            }
            else
            {
                return string.Format("Day {0}", dayNumer);
            }
        }
    }

    #region Activities

    [Table("Activities")]
    public abstract class Activity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual Trip Trip { get; set; }
        public int? BeginDay { get; set; }
        public TimeSpan? BeginTime { get; set; }
        public int? EndDay { get; set; }
        public TimeSpan? EndTime { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public string TagString { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        public abstract string Name { get; }
        public abstract string ActivityTypeName { get; }

        public string NiceName { get { return !string.IsNullOrWhiteSpace(Title) ? Title : Name; } }
        public bool IsUnscheduled { get { return !BeginDay.HasValue && !EndDay.HasValue; } }
    }

    [Table("TransportationActivities")]
    public class TransportationActivity : Activity
    {
        public virtual TransportationType TransportationType { get; set; }
        public virtual Place FromPlace { get; set; }
        public virtual Place ToPlace { get; set; }

        public override string Name
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (TransportationType != null) { sb.AppendFormat(TransportationType.Name); }
                else { sb.Append("Transportation"); }

                if (FromPlace != null) sb.AppendFormat(" From {0}", FromPlace.Name);
                if (ToPlace != null) sb.AppendFormat(" To {0}", ToPlace.Name);

                return sb.ToString();
            }
        }

        public override string ActivityTypeName { get { return "transportation"; } }
    }

    [Table("HotelActivities")]
    public class HotelActivity : Activity
    {
        public virtual Hotel Hotel { get; set; }

        public override string Name
        {
            get { return "Lodging at " + Hotel.Name; }
        }

        public override string ActivityTypeName { get { return "hotel"; } }
    }

    [Table("PlaceActivities")]
    public class PlaceActivity : Activity
    {
        public virtual Place Place { get; set; }

        public override string Name
        {
            get { return (Tags.Any() ? String.Join(", ", Tags.Select(t => t.Name)) + " at " : string.Empty) + Place.Name; }
        }

        public override string ActivityTypeName { get { return "place"; } }
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
        public virtual User User { get; set; }
        public DateTime Created_On { get; set; }
        public string Text { get; set; }
        public bool Public { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool ShowInSearch { get; set; }
        public DateTime? ModeratedOnUTC { get; set; }

        //public virtual ICollection<AmazonItem> AmazonItems { get; set; }

        public virtual ICollection<ItemTag> ItemTags { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }

        // Needed to make Html.ListBoxFor work in the admin section.
        public override string ToString() { return Id.ToString(); }

        public string NiceName { get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Name); } }

        public IEnumerable<Trip> PublicTrips
        {
            get
            {
                var trips = Activities.Select(a => a.Trip).Distinct().Where(t => t.ShowInSearch);
                return trips;
            }
        }
    }

    public class Item
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<ItemTag> ItemTags { get; set; }
    }

    public class ItemTag
    {
        public int Id { get; set; }
        public virtual Item Item { get; set; }
        public virtual Tag Tag { get; set; }
        public bool ShowInSearch { get; set; }
        public DateTime? ModeratedOnUTC { get; set; }
        public virtual ICollection<PackingListItem> PackingListItems { get; set; }
    }

    public class PackingListItem
    {
        public int Id { get; set; }
        public virtual ItemTag ItemTag { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Place Place { get; set; }
        public string Note { get; set; }
        public int Visibility_Id { get; set; }
        public string TagString { get; set; }

        public Visibility Visibility
        {
            get { return (Visibility)Visibility_Id; }
            set { Visibility_Id = (int)value; }
        }
    }

    #region Visibility

    public enum Visibility
    {
        Public = 0,
        Private = 1
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

        public IEnumerable<Trip> Trips { get { return HotelActivities.Select(a => a.Trip).Where(t => t.ShowInSearch).Distinct(); } }

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

        public string GoogId { get; set; }
        public string GoogReference { get; set; }

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

        public virtual ICollection<PlaceActivity> PlaceActivities { get; set; }
        public virtual ICollection<TransportationActivity> FromTransportationActivities { get; set; }
        public virtual ICollection<TransportationActivity> ToTransportationActivities { get; set; }

        public IEnumerable<Trip> PublicTrips
        {
            get
            {
                var placeActivityTrips = PlaceActivities.Select(pa => pa.Trip);
                var fromTransportationActivityTrips = FromTransportationActivities.Select(pa => pa.Trip);
                var toTransportationActivityTrips = ToTransportationActivities.Select(pa => pa.Trip);
                return placeActivityTrips.Union(fromTransportationActivityTrips).Union(toTransportationActivityTrips).Distinct().Where(t => t.ShowInSearch);
            }
        }

        public string NiceAddress
        {
            get
            {
                var parts = new[] { Address, Locality, Region, Country };
                var full = String.Join(", ", parts);
                return full;
            }
        }
    }
}