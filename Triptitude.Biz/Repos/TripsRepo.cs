using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects.DataClasses;
using System.Linq;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TripsRepo : Repo<Trip>
    {
        public IQueryable<Trip> Search(TripSearchForm form)
        {
            IQueryable<Trip> trips;

            if (!string.IsNullOrWhiteSpace(form.GoogReference))
            {
                var placesRepo = new PlacesRepo();
                var place = placesRepo.FindOrInitializeByGoogReference(form.GoogId, form.GoogReference);

                int radiusInMiles = 20;
                int radiusInMeters = (int)(radiusInMiles * 1609.344);
                const string sql = @"select t.*
from PlacesNear(@p0,@p1,@p2) pn
join ActivityPlaces ap on pn.Place_Id = ap.Place_Id
join Activities a on ap.Activity_Id = a.Id
join Trips t on a.Trip_Id = t.Id";
                trips = Sql(sql, place.Latitude, place.Longitude, radiusInMeters).AsQueryable();
            }
            else
            {
                var tripsRepo = new TripsRepo();
                trips = tripsRepo.FindAll();
            }

            if (!string.IsNullOrWhiteSpace(form.TagString))
            {
                var tagsRepo = new TagsRepo();
                var tag = tagsRepo.FindOrInitializeByName(form.TagString);
                trips = trips.Where(t => t.Activities.Where(a => !a.Deleted).Any(a => a.Tags.Select(ta => ta.Id).Contains(tag.Id)));
            }

            // only return trips that have at least 1 UserTrip that is non-deleted and public.
            trips = trips.Where(t => t.UserTrips.Any(ut => !ut.Deleted && ut.Visibility == (byte)UserTrip.UserTripVisibility.Public));

            return trips.Distinct();
        }

        //public Trip Save(CreateTripForm form, User currentUser)
        //{
        //    var placesRepo = new PlacesRepo();
        //    var activityPlacesRepo = new ActivityPlacesRepo();
        //    Place to = placesRepo.FindOrInitializeByGoogReference(form.ToGoogId, form.ToGoogReference);

        //    Trip trip = new Trip
        //    {
        //        Name = "My trip to " + to.Name,
        //        Created_On = DateTime.UtcNow,
        //        Activities = new List<Activity>(),
        //        UserTrips = new Collection<UserTrip>()
        //    };
        //    UserTrip userTrip = new UserTrip
        //                            {
        //                                Trip = trip,
        //                                IsCreator = true,
        //                                Created_On = DateTime.UtcNow,
        //                                User = currentUser,
        //                                Visibility = (byte)form.Visibility,
        //                                Guid = Guid.NewGuid()
        //                            };
        //    trip.UserTrips.Add(userTrip);

        //    var activity = new Activity { BeginDay = 1, EndDay = 1, ActivityPlaces = new EntityCollection<ActivityPlace>(), IsTransportation = true };
        //    activityPlacesRepo.FindOrInitialize(activity, 1, to);
        //    trip.Activities.Add(activity);

        //    // TODO: don't create a new one, use this one
        //    var tripsRepo = new TripsRepo();
        //    tripsRepo.Add(trip);
        //    tripsRepo.Save();

        //    ActivitiesRepo.UpdateGeoPoints(activity);

        //    return trip;
        //}

        public Trip Save(NewCreateTripForm form, User creator)
        {
            Trip trip = new Trip
                            {
                                Name = form.Name,
                                Created_On = DateTime.UtcNow,
                                Activities = new List<Activity>(),
                                UserTrips = new Collection<UserTrip>()
                            };
            UserTrip userTrip = new UserTrip { Trip = trip, IsCreator = true, Created_On = DateTime.UtcNow, User = creator };
            trip.UserTrips.Add(userTrip);
            Add(trip);
            Save();
            return trip;
        }
    }
}