using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using Triptitude.Biz.Extensions;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class ActivitiesRepo : Repo<Activity>
    {
        public override void Delete(Activity entity)
        {
            entity.Deleted = true;
        }

        private static void SetBaseProperties(Activity activity, ActivityForm form, User currentUser)
        {
            activity.Trip = new TripsRepo().Find(form.TripId);
            activity.Title = string.IsNullOrWhiteSpace(form.Title) ? null : form.Title;
            activity.BeginDay = form.BeginDay;
            activity.BeginTime = string.IsNullOrWhiteSpace(form.BeginTime) ? (TimeSpan?)null : DateTime.Parse(string.Format("{0} {1}", DateTime.Today.ToShortDateString(), form.BeginTime)).TimeOfDay;
            activity.EndDay = form.EndDay;
            activity.EndTime = string.IsNullOrWhiteSpace(form.EndTime) ? (TimeSpan?)null : DateTime.Parse(string.Format("{0} {1}", DateTime.Today.ToShortDateString(), form.EndTime)).TimeOfDay;
            activity.TagString = form.TagString;

            if (activity.Tags != null) activity.Tags.Clear();
            if (!string.IsNullOrWhiteSpace(form.TagString))
            {
                activity.Tags = new TagsRepo().FindOrInitializeAll(form.TagString).ToList();
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
                activity = new TransportationActivity
                {
                    ActivityPlaces = new EntityCollection<ActivityPlace>()
                };
                Add(activity);
            }

            SetBaseProperties(activity, form, currentUser);

            if (form.TransportationTypeId.HasValue)
            {
                var type = new TransportationTypesRepo().Find(form.TransportationTypeId.Value);
                activity.TransportationType = type;
            }
            else
            {
                // because EF4 is stupid
                var tmp = activity.TransportationType;
                activity.TransportationType = null;
            }

            var placesRepo = new PlacesRepo();
            if (string.IsNullOrWhiteSpace(form.FromGoogReference))
            {
                var activityPlace = activity.ActivityPlaces.FirstOrDefault(ap => ap.SortIndex == 0);
                if (activityPlace != null) new ActivityPlacesRepo().Delete(activityPlace);
            }
            else
            {
                var place = placesRepo.FindOrInitializeByGoogReference(form.FromGoogId, form.FromGoogReference);
                new ActivityPlacesRepo().FindOrInitialize(activity, 0, place);
            }

            if (string.IsNullOrWhiteSpace(form.ToGoogReference))
            {
                var activityPlace = activity.ActivityPlaces.FirstOrDefault(ap => ap.SortIndex == 1);
                if (activityPlace != null) new ActivityPlacesRepo().Delete(activityPlace);
            }
            else
            {
                var place = placesRepo.FindOrInitializeByGoogReference(form.ToGoogId, form.ToGoogReference);
                new ActivityPlacesRepo().FindOrInitialize(activity, 1, place);
            }

            Save();
            UpdateGeoPoints(activity);
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
                activity.ActivityPlaces = new EntityCollection<ActivityPlace>();
                Add(activity);
            }

            SetBaseProperties(activity, form, currentUser);

            PlacesRepo placesRepo = new PlacesRepo();

            if (string.IsNullOrWhiteSpace(form.GoogReference))
            {
                var activityPlace = activity.ActivityPlaces.FirstOrDefault();
                if (activityPlace != null) new ActivityPlacesRepo().Delete(activityPlace);
            }
            else
            {
                var place = placesRepo.FindOrInitializeByGoogReference(form.GoogId, form.GoogReference);
                new ActivityPlacesRepo().FindOrInitialize(activity, 0, place);
            }

            if (activity.Place == null && activity.Tags.IsNullOrEmpty() && string.IsNullOrEmpty(activity.Title))
            {
                Delete(activity);
            }

            Save();
            UpdateGeoPoints(activity);
            return activity;
        }

        public static void UpdateGeoPoints(Activity activity)
        {
            if (activity.ActivityPlaces.IsNullOrEmpty()) return;

            // Needs an array of strings
            var placeIds = activity.ActivityPlaces.Select(ap => ap.Place.Id.ToString()).ToArray();
            new Repo().ExecuteSql("update Places set GeoPoint = geography::Point(Latitude, Longitude, 4326) where Latitude is not null and Longitude is not null and Id in (@p0)", placeIds);
        }
    }
}