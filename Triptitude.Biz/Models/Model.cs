using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Models
{
    public class User
    {
        public User()
        {
            Notes = new Collection<Note>();
            UserTrips = new Collection<UserTrip>();
        }

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
        public virtual ICollection<History> Histories { get; set; }

        public bool IsAdmin { get { return Email == "mikecomstock@gmail.com"; } }

        public IEnumerable<Trip> Trips(User forUser)
        {
            var a = UserTrips.Where(ut => !ut.Deleted);
            a = a.Where(ut => ut.Visibility == (byte)UserTrip.UserTripVisibility.Public || forUser.OwnsTrips(ut.Trip));
            var trips = a.Select(ut => ut.Trip);
            trips = trips.OrderByDescending(t => t.Id);
            return trips;
        }

        public string FullName
        {
            get
            {
                return string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
                           ? "anonymous user"
                           : string.Format("{0} {1}", FirstName, LastName).Trim();
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

        public string PhotoURL
        {
            get
            {
                var email = string.IsNullOrWhiteSpace(Email) ? "anon@triptitude.com" : Email;
                return "http://www.gravatar.com/avatar/" + email.Trim().ToLower().Md5Hash() + "?d=mm";
            }
        }

        public string PhotoURLSmall
        {
            get { return PhotoURL + "&s=50"; }
        }

        public dynamic Json(User forUser, UrlHelper url)
        {
            return new
                       {
                           ID = Id,
                           Email,
                           DefaultTripID = DefaultTrip == null ? null : (int?)DefaultTrip.Id,
                           PhotoURL,
                           Trips = Trips(forUser).Select(t => t.Json(forUser, url))
                       };
        }
    }

    public class History
    {
        public int Id { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public virtual User User { get; set; }
        public virtual Trip Trip { get; set; }
        public byte Action { get; set; }
        public int? TableId1 { get; set; }
        public int? TableId2 { get; set; }

        public HistoryAction HistoryAction { get { return (HistoryAction)Enum.Parse(typeof(HistoryAction), Action.ToString()); } }

        public string ToString(User forUser)
        {
            var usersRepo = new UsersRepo();
            var activitiesRepo = new ActivitiesRepo();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} - ", CreatedOnUTC.ToRelative());
            sb.AppendFormat("{0} ", User == forUser ? "You" : User.FullName);

            switch (HistoryAction)
            {
                case HistoryAction.CreatedTrip:
                    sb.AppendFormat("created the trip");
                    break;
                case HistoryAction.UpdatedTrip:
                    sb.AppendFormat("update trip settings");
                    break;
                case HistoryAction.DeletedTrip:
                    sb.AppendFormat("deleted the trip");
                    break;
                case HistoryAction.AddUserToTrip:
                    {
                        var user = usersRepo.Find(TableId1.Value);
                        sb.AppendFormat("added {0}", user.FullName);
                        break;
                    }
                case HistoryAction.CreateInvitation:
                    {
                        var user = usersRepo.Find(TableId1.Value);
                        sb.AppendFormat("invited {0} to help plan", user.FullName);
                        break;
                    }
                case HistoryAction.AcceptInvitation:
                    {
                        sb.AppendFormat("accepted the invitation");
                        break;
                    }
                case HistoryAction.RemoveUserFromTrip:
                    {
                        var user = usersRepo.Find(TableId1.Value);
                        sb.AppendFormat("removed {0}", user.FullName);
                        break;
                    }
                case HistoryAction.CreateActivity:
                    {
                        var activity = activitiesRepo.Find(TableId1.Value);
                        sb.AppendFormat("added \"{0}\"", activity.Title);
                        break;
                    }
                case HistoryAction.UpdateActivity:
                    {
                        var activity = activitiesRepo.Find(TableId1.Value);
                        sb.AppendFormat("updated \"{0}\"", activity.Title);
                        break;
                    }
                case HistoryAction.DeletedActivity:
                    {
                        var activity = activitiesRepo.Find(TableId1.Value);
                        sb.AppendFormat("removed \"{0}\"", activity.Title);
                        break;
                    }
                case HistoryAction.CreatedNote:
                    {
                        var notesRepo = new Repo<Note>();
                        var note = notesRepo.Find(TableId1.Value);
                        sb.AppendFormat("added a note: {0}", note.Text);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return sb.ToString().Trim();
        }
    }

    public enum HistoryAction : byte
    {
        CreatedTrip = 1,
        UpdatedTrip = 2,
        DeletedTrip = 3,
        AddUserToTrip = 4,
        CreateInvitation = 5,
        RemoveUserFromTrip = 6,
        CreateActivity = 7,
        UpdateActivity = 8,
        DeletedActivity = 9,
        AcceptInvitation = 10,
        CreatedNote = 11
    }

    public class UserTrip
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Trip Trip { get; set; }
        public bool IsCreator { get; set; }
        public bool Deleted { get; set; }
        public byte Visibility { get; set; }
        public DateTime? Created_On { get; set; }
        public Guid? Guid { get; set; }
        public virtual ICollection<EmailInvite> EmailInvites { get; set; }

        public enum UserTripVisibility : byte
        {
            Public = 0,
            Private = 1
        }

        public string InvitationURL(UrlHelper url)
        {
            if (Guid.HasValue)
                return url.Action("Details", "Invitations", new { guid = Guid }, "http");
            else return string.Empty;
        }

        public dynamic Json(User forUser, UrlHelper url)
        {
            return new
                       {
                           id = Id,
                           User.FirstName,
                           User.LastName,
                           User.FullName,
                           User.PhotoURLSmall,
                           UserId = User.Id,
                           InvitationURL = InvitationURL(url),

                       };
        }
    }

    public class EmailInvite
    {
        public int Id { get; set; }
        public virtual UserTrip UserTrip { get; set; }
        public string Email { get; set; }
        public DateTime Created_On { get; set; }

        public dynamic Json(User forUser)
        {
            if (!forUser.OwnsTrips(UserTrip.Trip)) return new { };
            return new
                       {
                           id = Id,
                           Email,
                           UserTrip_Id = UserTrip.Id
                       };
        }
    }

    public class Trip
    {
        public Trip()
        {
            Activities = new Collection<Activity>();
            UserTrips = new Collection<UserTrip>();
            Histories = new Collection<History>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<UserTrip> UserTrips { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public DateTime Created_On { get; set; }
        public User Creator { get { return UserTrips.First(ut => ut.IsCreator).User; } }
        public DateTime? BeginDate { get; set; }
        public bool ShowInSearch { get; set; }
        public DateTime? ModeratedOnUTC { get; set; }

        public IEnumerable<UserTrip> UserTripsFor(User user)
        {
            // Only return users that haven't deleted the trip
            var a = UserTrips.Where(ut => !ut.Deleted);
            // If the user owns the trip, return them all. Otherwise just return public ones.
            a = a.Where(ut => ut.Visibility == (byte)UserTrip.UserTripVisibility.Public || user.OwnsTrips(ut.Trip));
            return a;
        }

        public IEnumerable<User> Users { get { return UserTrips.Select(ut => ut.User); } }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<PackingListItem> PackingListItems { get; set; }
        public virtual ICollection<History> Histories { get; set; }

        public IEnumerable<Activity> Ordered
        {
            get { return Activities.OrderBy(a => a.OrderNumber); }
        }

        public IEnumerable<Activity> NonDeletedActivities
        {
            get { return Activities.Where(a => !a.Deleted).OrderBy(a => a.OrderNumber); }
        }

        public bool Sharable { get { return UserTrips.Any(ut => ut.Visibility == (byte)UserTrip.UserTripVisibility.Public); } }

        public IEnumerable<DateTime?> Dates
        {
            get
            {
                var beginAts = NonDeletedActivities.Select(a => a.BeginAt);
                var withValues = beginAts.Where(d => d.HasValue);
                if (withValues.Any())
                {
                    var min = withValues.Min(d => d.Value.Date);
                    var max = withValues.Max(d => d.Value.Date);

                    for (DateTime i = min; i <= max; i = i.AddDays(1))
                    {
                        yield return i;
                    }
                }
                // Take care of 'unscheduled' activities
                if (beginAts.Any(d => !d.HasValue))
                    yield return null;
            }
        }

        public IEnumerable<Activity> ActivitiesOn(int? dayNumber)
        {
            return dayNumber.HasValue
                ? Activities.Where(a => !a.Deleted && (a.BeginDay == dayNumber || a.EndDay == dayNumber))
                : Activities.Where(a => !a.Deleted && !a.BeginDay.HasValue && !a.EndDay.HasValue);
        }

        public IEnumerable<Activity> ActivitiesOnDate(DateTime? date)
        {
            if (!date.HasValue)
                return NonDeletedActivities.Where(a => !a.BeginAt.HasValue);
            else
            {
                return NonDeletedActivities.Where(a => a.BeginAt.HasValue && a.BeginAt.Value.Date == date);
            }
        }

        public IEnumerable<Tag> Tags
        {
            get
            {
                var tags = Activities.Where(a => !a.Deleted).SelectMany(a => a.Tags).Distinct();
                return tags;
            }
        }

        public void AddHistory(User user, HistoryAction action, int? tableId1 = null, int? tableId2 = null)
        {
            History h = new History
                            {
                                User = user,
                                Action = (byte)action,
                                CreatedOnUTC = DateTime.UtcNow,
                                TableId1 = tableId1,
                                TableId2 = tableId2,
                            };
            Histories.Add(h);
        }

        public Note AddNote(User user, Activity activity, string text)
        {
            var note = new Note
                           {
                               Activity = activity,
                               User = user,
                               Created_On = DateTime.UtcNow,
                               Text = text.Trim()
                           };
            Notes.Add(note);
            return note;
        }

        public dynamic Json(User forUser, UrlHelper url)
        {
            return new
            {
                ID = Id,
                Name,
                Activities = NonDeletedActivities.OrderBy(a => a.BeginAt).ThenBy(a => a.OrderNumber).Select(a => a.Json(forUser, url))
            };
        }
    }

    #region Activities

    [Table("Activities")]
    public class Activity
    {
        public Activity()
        {
            ActivityPlaces = new Collection<ActivityPlace>();
            Notes = new Collection<Note>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual Trip Trip { get; set; }
        public int? BeginDay { get; set; }
        public DateTime? BeginAt { get; set; }
        public TimeSpan? BeginTime { get; set; }
        public int? EndDay { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTime? EndAt { get; set; }
        public int OrderNumber { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public string TagString { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<ActivityPlace> ActivityPlaces { get; set; }
        public bool Deleted { get; set; }

        public bool IsTransportation { get; set; }
        public virtual TransportationType TransportationType { get; set; }
        public string SourceURL { get; set; }

        public bool IsUnscheduled { get { return !BeginDay.HasValue && !EndDay.HasValue; } }

        public TimeSpan? TimeForSort(int? day)
        {
            TimeSpan? result = null;
            if (day == BeginDay) result = BeginTime;
            else if (day == EndDay) result = EndTime;

            return result ?? TimeSpan.MaxValue;
        }

        //public string Duration
        //{
        //    get
        //    {
        //        if ((BeginDay.HasValue && !EndDay.HasValue) || (!BeginDay.HasValue && EndDay.HasValue)) return null;
        //        if ((BeginTime.HasValue && !EndTime.HasValue) || (!BeginTime.HasValue && EndTime.HasValue)) return null;

        //        DateTime begin = DateTime.Today;
        //        DateTime end = DateTime.Today;

        //        if (BeginDay.HasValue) begin = begin.AddDays(BeginDay.Value);
        //        if (EndDay.HasValue) end = end.AddDays(EndDay.Value);

        //        if (BeginTime.HasValue) begin = begin.Add(BeginTime.Value);
        //        if (EndTime.HasValue) end = end.Add(EndTime.Value);

        //        TimeSpan duration = end - begin;

        //        List<string> parts = new List<string>(3);
        //        if (duration.Days == 1) parts.Add(duration.Days + "day");
        //        if (duration.Days > 1) parts.Add(duration.Days + "days");
        //        if (duration.Hours == 1) parts.Add(duration.Hours + " hour");
        //        if (duration.Hours > 1) parts.Add(duration.Hours + " hours");
        //        if (duration.Minutes == 1) parts.Add(duration.Minutes + " minute");
        //        if (duration.Minutes > 1) parts.Add(duration.Minutes + " minutes");

        //        return string.Join(", ", parts);
        //    }
        //}

        public dynamic Json(User forUser, UrlHelper url)
        {
            return new
                       {
                           ID = Id,
                           Title,
                           IsTransportation,
                           BeginAt = BeginAt.HasValue ? DateTime.SpecifyKind(BeginAt.Value, DateTimeKind.Utc) : (DateTime?)null,
                           //EndAt,
                           TransportationTypeName = TransportationType == null ? string.Empty : TransportationType.Name,
                           SourceURL,
                           TagString,
                           OrderNumber,
                           Trip = new
                                      {
                                          ID = Trip.Id,
                                          Trip.Name,
                                          UserOwnsTrip = forUser.OwnsTrips(Trip)
                                      },
                           Places = from p in ActivityPlaces
                                    select new
                                               {
                                                   p.SortIndex,
                                                   p.Place.Id,
                                                   p.Place.Name
                                               },
                           Notes = Notes.OrderByDescending(n => n.Id).Select(n => n.Json(forUser, url))
                       };
        }
    }

    public class ActivityPlace
    {
        public int Id { get; set; }
        public virtual Activity Activity { get; set; }
        public int SortIndex { get; set; }
        public virtual Place Place { get; set; }
    }

    //[Table("TransportationActivities")]
    //public class TransportationActivity : Activity
    //{
    //    public virtual TransportationType TransportationType { get; set; }

    //    public Place FromPlace
    //    {
    //        get
    //        {
    //            var activityPlace = ActivityPlaces.FirstOrDefault(ap => ap.SortIndex == 0);
    //            return activityPlace != null ? activityPlace.Place : null;
    //        }
    //    }
    //    public Place ToPlace
    //    {
    //        get
    //        {
    //            var activityPlace = ActivityPlaces.FirstOrDefault(ap => ap.SortIndex == 1);
    //            return activityPlace != null ? activityPlace.Place : null;
    //        }
    //    }

    //    public override string Name
    //    {
    //        get
    //        {
    //            StringBuilder sb = new StringBuilder();
    //            if (TransportationType != null) { sb.AppendFormat(TransportationType.Name); }
    //            else { sb.Append("Transportation"); }

    //            if (FromPlace != null) sb.AppendFormat(" From {0}", FromPlace.Name);
    //            if (ToPlace != null) sb.AppendFormat(" To {0}", ToPlace.Name);

    //            return sb.ToString();
    //        }
    //    }

    //    public override string ActivityTypeName { get { return "transportation"; } }
    //}

    //[Table("PlaceActivities")]
    //public class PlaceActivity : Activity
    //{
    //    public Place Place
    //    {
    //        get
    //        {
    //            var activityPlace = ActivityPlaces.FirstOrDefault();
    //            return activityPlace != null ? activityPlace.Place : null;
    //        }
    //    }

    //    public override string Name
    //    {
    //        get { return (Tags.Any() ? String.Join(", ", Tags.Select(t => t.Name)) + " at " : string.Empty) + Place.Name; }
    //    }

    //    public override string ActivityTypeName { get { return "place"; } }
    //}

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
        public virtual Trip Trip { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual User User { get; set; }
        public DateTime Created_On { get; set; }
        public string Text { get; set; }

        public dynamic Json(User forUser, UrlHelper url)
        {
            return new
                       {
                           Id,
                           User = new
                                      {
                                          User.FullName,
                                          DetailsURL = url.Details(User)
                                      },
                           Text,
                           RelativeTime = Created_On.ToRelative()
                       };
        }
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

        public string StaticMapURL()
        {
            if (!Latitude.HasValue || !Longitude.HasValue) return null;

            var s = "http://maps.googleapis.com/maps/api/staticmap?size=200x200&zoom=14&markers={0},{1}&sensor=true";
            return string.Format(s, Latitude.Value, Longitude.Value);
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