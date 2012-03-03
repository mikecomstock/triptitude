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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string AnonymousId { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? GuidCreatedOnUtc { get; set; }

        public virtual Trip DefaultTrip { get; set; }
        public virtual ICollection<UserTrip> UserTrips { get; set; }
        public virtual ICollection<Note> Notes { get; set; }

        public IEnumerable<Trip> Trips { get { return UserTrips == null ? null : UserTrips.Select(ut => ut.Trip); } }

        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName)) return "anonymous user";
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
        public string FirstNameLastInitial
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName)) return "anonymous user";
                return string.Format("{0} {1}.", FirstName, LastName[0]);
            }
        }

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
            var userOwnsAllTrips = trips.All(t => t.Users.Contains(this));
            return userOwnsAllTrips;
        }

        public bool CreatedTrips(params Trip[] trips)
        {
            var userCreatedAllTrips = trips.All(t => t.Creator == this);
            return userCreatedAllTrips;
        }

        public IEnumerable<Item> DefaultTripItems
        {
            get
            {
                return DefaultTrip == null
                    ? new List<Item>()
                    : DefaultTrip.PackingListItems.Select(pli => pli.ItemTag.Item).Distinct();
            }
        }
    }

    public class History
    {
        public int Id { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public User User { get; set; }
        public Trip Trip { get; set; }
        public byte Action { get; set; }
        public byte TableName { get; set; }
        public int TableId { get; set; }

        public HistoryTable HistoryTable { get { return (HistoryTable)Enum.Parse(typeof(HistoryTable), TableName.ToString()); } }
        public HistoryAction HistoryAction { get { return (HistoryAction)Enum.Parse(typeof(HistoryAction), Action.ToString()); } }
    }

    public enum HistoryAction : byte
    {
        Created = 0,
        Modified = 1,
        Deleted = 2
    }

    public enum HistoryTable : byte
    {
        Trips = 0, Activities = 1, PackingListItems = 2
    }

    public class UserTrip
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Trip Trip { get; set; }
        public bool IsCreator { get; set; }
        public byte Status { get; set; }
        public bool Archived { get; set; }
        public DateTime StatusUpdatedOnUTC { get; set; }
    }

    public enum UserTripStatus : byte
    {
        Attending = 1,
        Invited = 2,
        NotAttending = 3
    }

    public class Trip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<UserTrip> UserTrips { get; set; }
        public DateTime Created_On { get; set; }
        public User Creator { get { return UserTrips.First(ut => ut.IsCreator).User; } }
        public DateTime? BeginDate { get; set; }
        public bool ShowInSearch { get; set; }
        public DateTime? ModeratedOnUTC { get; set; }
        public byte Visibility { get; set; }

        public enum TripVisibility : byte
        {
            Public = 0,
            Private = 1
        }

        public IEnumerable<User> Users { get { return UserTrips.Select(ut => ut.User); } }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<PackingListItem> PackingListItems { get; set; }
        public virtual ICollection<History> Histories { get; set; }

        public int TotalDays
        {
            get
            {
                int? max = Activities.Where(a => !a.Deleted).Select(a => a.BeginDay).Union(Activities.Where(a => !a.Deleted).Select(a => a.EndDay)).Max();
                return max ?? 1;
            }
        }

        public IEnumerable<Activity> ActivitiesOn(int? dayNumber)
        {
            return dayNumber.HasValue
                ? Activities.Where(a => !a.Deleted && (a.BeginDay == dayNumber || a.EndDay == dayNumber))
                : Activities.Where(a => !a.Deleted && !a.BeginDay.HasValue && !a.EndDay.HasValue);
        }

        public IEnumerable<Tag> Tags
        {
            get
            {
                var tags = Activities.Where(a => !a.Deleted).SelectMany(a => a.Tags).Distinct();
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
    public class Activity
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
        public virtual ICollection<ActivityPlace> ActivityPlaces { get; set; }
        public bool Deleted { get; set; }

        [NotMapped]
        public virtual string Name { get; set; }
        [NotMapped]
        public virtual string ActivityTypeName { get; set; }

        public string NiceName { get { return !string.IsNullOrWhiteSpace(Title) ? Title : Name; } }
        public bool IsUnscheduled { get { return !BeginDay.HasValue && !EndDay.HasValue; } }

        public TimeSpan? TimeForSort(int? day)
        {
            TimeSpan? result = null;
            if (day == BeginDay) result = BeginTime;
            else if (day == EndDay) result = EndTime;

            return result ?? TimeSpan.MaxValue;
        }

        public string Duration
        {
            get
            {
                if ((BeginDay.HasValue && !EndDay.HasValue) || (!BeginDay.HasValue && EndDay.HasValue)) return null;
                if ((BeginTime.HasValue && !EndTime.HasValue) || (!BeginTime.HasValue && EndTime.HasValue)) return null;

                DateTime begin = DateTime.Today;
                DateTime end = DateTime.Today;

                if (BeginDay.HasValue) begin = begin.AddDays(BeginDay.Value);
                if (EndDay.HasValue) end = end.AddDays(EndDay.Value);

                if (BeginTime.HasValue) begin = begin.Add(BeginTime.Value);
                if (EndTime.HasValue) end = end.Add(EndTime.Value);

                TimeSpan duration = end - begin;

                List<string> parts = new List<string>(3);
                if (duration.Days == 1) parts.Add(duration.Days + "day");
                if (duration.Days > 1) parts.Add(duration.Days + "days");
                if (duration.Hours == 1) parts.Add(duration.Hours + " hour");
                if (duration.Hours > 1) parts.Add(duration.Hours + " hours");
                if (duration.Minutes == 1) parts.Add(duration.Minutes + " minute");
                if (duration.Minutes > 1) parts.Add(duration.Minutes + " minutes");

                return string.Join(", ", parts);
            }
        }
    }

    public class ActivityPlace
    {
        public int Id { get; set; }
        public virtual Activity Activity { get; set; }
        public int SortIndex { get; set; }
        public virtual Place Place { get; set; }
    }

    [Table("TransportationActivities")]
    public class TransportationActivity : Activity
    {
        public virtual TransportationType TransportationType { get; set; }

        public Place FromPlace
        {
            get
            {
                var activityPlace = ActivityPlaces.FirstOrDefault(ap => ap.SortIndex == 0);
                return activityPlace != null ? activityPlace.Place : null;
            }
        }
        public Place ToPlace
        {
            get
            {
                var activityPlace = ActivityPlaces.FirstOrDefault(ap => ap.SortIndex == 1);
                return activityPlace != null ? activityPlace.Place : null;
            }
        }

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

    [Table("PlaceActivities")]
    public class PlaceActivity : Activity
    {
        public Place Place
        {
            get
            {
                var activityPlace = ActivityPlaces.FirstOrDefault();
                return activityPlace != null ? activityPlace.Place : null;
            }
        }

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

        public IEnumerable<Item> Items
        {
            get
            {
                var items = ItemTags.Where(it => it.ShowInSearch).OrderByDescending(it => it.PackingListItems.Count()).Select(it => it.Item);
                return items;
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

        public virtual ICollection<ActivityPlace> ActivityPlaces { get; set; }

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

    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Category { get; set; }
        public DateTime DisplayDate { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime UpdatedOnUTC { get; set; }
        public virtual User User { get; set; }
    }
}