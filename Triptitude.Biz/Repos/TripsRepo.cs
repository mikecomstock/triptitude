using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TripsRepo : Repo<Trip>
    {
        public IEnumerable<Trip> Search(TripSearchForm form)
        {
            throw new NotImplementedException();
            //            int radiusInMeters = (int)(form.RadiusInMiles * 1609.344);

            //            const string sql = @"select distinct * from (
            //select t.* from PlacesNear(@p0,@p1,@p2) pn
            //join PlaceActivities pa on pn.place_id = pa.Place_Id
            //join Activities a on pa.Id = a.Id
            //join Trips t on a.Trip_Id = t.Id
            //union
            //select t.* from HotelsNear(@p0,@p1,@p2) hn
            //join HotelActivities ha on hn.Hotel_Id = ha.Hotel_Id
            //join Activities a on ha.Id = a.Id
            //join Trips t on a.Trip_Id = t.Id
            //) a";
            //            var trips = Sql(sql, form.Latitude, form.Longitude, radiusInMeters);
            //            return trips;
        }

        public Trip Save(CreateTripForm form, User currentUser)
        {
            TransportationType transportationType = null;
            var placesRepo = new PlacesRepo();
            var activityPlacesRepo = new ActivityPlacesRepo();
            Place to = placesRepo.FindOrInitializeByGoogReference(form.ToGoogId, form.ToGoogReference);

            Trip trip = new Trip { Name = "My trip to " + to.Name, User = currentUser, Created_On = DateTime.UtcNow, Activities = new List<Activity>() };
            var activity = new TransportationActivity { BeginDay = 1, EndDay = 1, TransportationType = transportationType, ActivityPlaces = new EntityCollection<ActivityPlace>() };
            activityPlacesRepo.FindOrInitialize(activity, 1, to);
            trip.Activities.Add(activity);

            var tripsRepo = new TripsRepo();
            tripsRepo.Add(trip);
            tripsRepo.Save();

            ActivitiesRepo.UpdateGeoPoints(activity);

            return trip;
        }

        public TripSettingsForm GetSettingsForm(Trip trip)
        {
            var form = new TripSettingsForm
                           {
                               Name = trip.Name,
                               BeginDate = trip.BeginDate.HasValue ? trip.BeginDate.Value.ToString("MM/dd/yyyy") : string.Empty
                           };
            return form;
        }

        public void Save(Trip trip, TripSettingsForm form)
        {
            trip.Name = form.Name;

            DateTime parsedDate;
            if (DateTime.TryParse(form.BeginDate, out parsedDate))
                trip.BeginDate = parsedDate;
            else
                trip.BeginDate = null;

            Save();
        }
    }
}