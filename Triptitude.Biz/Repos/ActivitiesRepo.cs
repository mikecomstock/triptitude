using System;
using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class ActivitiesRepo : Repo<Activity>
    {
        private static void SetBaseProperties(Activity activity, ActivityForm form, User currentUser)
        {
            activity.Trip = new TripsRepo().Find(form.TripId);
            activity.BeginDay = form.BeginDay.Value;
            activity.EndDay = form.EndDay.Value;

            if (activity.Tags != null)
            {
                foreach (var tag in activity.Tags.ToList())
                {
                    activity.Tags.Remove(tag);
                }
            }

            if (!string.IsNullOrWhiteSpace(form.TagName))
            {
                Tag tag = new TagsRepo().FindOrInitializeByName(form.TagName);
                activity.Tags = new List<Tag> { tag };
            }

            if (!string.IsNullOrWhiteSpace(form.Note))
            {
                Note note = new Note
                {
                    Text = form.Note.Trim(),
                    Created_On = DateTime.UtcNow,
                    User = currentUser
                };

                if (activity.Notes == null) { activity.Notes = new List<Note>(); }
                activity.Notes.Add(note);
            }
        }

        public Activity Save(TransportationActivityForm form, User currentUser)
        {
            TransportationActivity activity;

            if (form.ActivityId.HasValue)
            {
                activity = (TransportationActivity)Find(form.ActivityId.Value);
            }
            else
            {
                activity = new TransportationActivity();
                Add(activity);
            }

            SetBaseProperties(activity, form, currentUser);

            var type = new TransportationTypesRepo().Find(form.TransportationTypeId);
            activity.TransportationType = type;

            var citiesRepo = new CitiesRepo();
            activity.FromCity = citiesRepo.Find(form.FromCityId);
            activity.ToCity = citiesRepo.Find(form.ToCityId);

            Save();

            return activity;
        }

        public Activity Save(HotelActivityForm form, User currentUser)
        {
            HotelActivity activity;

            if (form.ActivityId.HasValue)
            {
                activity = (HotelActivity)Find(form.ActivityId.Value);
            }
            else
            {
                activity = new HotelActivity();
                Add(activity);
            }

            SetBaseProperties(activity, form, currentUser);

            HotelsRepo hotelsRepo = new HotelsRepo();
            activity.Hotel = hotelsRepo.Find(form.HotelId);

            Save();

            return activity;
        }

        public Activity Save(PlaceActivityForm form, User currentUser)
        {
            PlaceActivity activity;

            if (form.ActivityId.HasValue)
            {
                activity = (PlaceActivity)Find(form.ActivityId.Value);
            }
            else
            {
                activity = new PlaceActivity();
                Add(activity);
            }

            SetBaseProperties(activity, form, currentUser);

            PlacesRepo placesRepo = new PlacesRepo();

            // If this is a custom place:
            if (string.IsNullOrWhiteSpace(form.FactualId))
            {
                Place place = form.PlaceId.HasValue ? placesRepo.Find(form.PlaceId.Value) : new Place();
                place.Name = form.Name;
                place.Address = form.Address;
                place.Telephone = form.Telephone;
                place.Website = form.Website;
                place.Latitude = string.IsNullOrWhiteSpace(form.Latitude) ? (decimal?)null : decimal.Parse(form.Latitude);
                place.Longitude = string.IsNullOrWhiteSpace(form.Longitude) ? (decimal?)null : decimal.Parse(form.Longitude);
                activity.Place = place;
            }
            else
            {
                activity.Place = placesRepo.FindOrInitializeByFactualId(form.FactualId);
            }

            Save();
            new Repo().ExecuteSql("update Places set GeoPoint = geography::Point(Latitude, Longitude, 4326) where Latitude is not null and Longitude is not null and Id = @p0", activity.Place.Id);

            return activity;
        }
    }
}