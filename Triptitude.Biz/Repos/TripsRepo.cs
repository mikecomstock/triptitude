﻿using System;
using System.Collections.Generic;
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
                trips = trips.Where(t => t.Activities.Any(a => a.Tags.Select(ta => ta.Id).Contains(tag.Id)));
            }

            return trips.Distinct();
        }

        public Trip Save(CreateTripForm form, User currentUser)
        {
            TransportationType transportationType = null;
            var placesRepo = new PlacesRepo();
            var activityPlacesRepo = new ActivityPlacesRepo();
            Place to = placesRepo.FindOrInitializeByGoogReference(form.ToGoogId, form.ToGoogReference);

            Trip trip = new Trip { Name = "My trip to " + to.Name, Created_On = DateTime.UtcNow, Activities = new List<Activity>() };
            UserTrip userTrip = new UserTrip { Trip = trip, IsCreator = true, Status = (byte)UserTripStatus.Attending, StatusUpdatedOnUTC = DateTime.UtcNow };
            currentUser.UserTrips.Add(userTrip);

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