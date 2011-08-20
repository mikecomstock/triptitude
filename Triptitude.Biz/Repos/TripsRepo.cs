﻿using System;
using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TripsRepo : Repo<Trip>
    {
        public IEnumerable<Trip> Search(TripSearchForm form)
        {
            int radiusInMeters = (int)(form.RadiusInMiles * 1609.344);

            const string sql = @"select distinct t.* from PlacesNear(@p0,@p1,@p2) pn
join PlaceActivities pa on pn.place_id = pa.Place_Id
join Activities a on pa.Id = a.Id
join Trips t on a.Trip_Id = t.Id
";
            var trips = Sql(sql, form.Latitude, form.Longitude, radiusInMeters);
            return trips;
        }

        public Trip Save(CreateTripForm form, User currentUser)
        {
            Trip trip = Generate(form, currentUser);
            _db.Trips.Add(trip);

            Save();

            return trip;
        }

        public Trip Generate(CreateTripForm form, User currentUser)
        {
            Random r = new Random();
            int minimumNightStay = form.NumDays < 7 ? 3 : form.NumDays < 14 ? 5 : form.NumDays < 30 ? 7 : 10;
            int hotelsNeeded = (int)Math.Ceiling((decimal)form.NumDays / minimumNightStay);

            var transportationTypesRepo = new TransportationTypesRepo();
            var fly = transportationTypesRepo.Find("Fly");

            var citiesRepo = new CitiesRepo();
            var fromCity = citiesRepo.Find(form.FromId.Value);
            var toCity = citiesRepo.Find(form.ToId.Value);

            Trip trip = new Trip { Name = "My trip to " + toCity.ShortName, User = currentUser, Created_On = DateTime.UtcNow, Activities = new List<Activity>() };

            // Transportation to destination
            trip.Activities.Add(new TransportationActivity { BeginDay = 1, EndDay = 1, TransportationType = fly, FromCity = fromCity, ToCity = toCity });

            // Hotels for each day
            var hotelsRepo = new HotelsRepo();
            var hotels = hotelsRepo.IncrementalSearch(new HotelSearchForm { Latitude = toCity.Latitude, Longitude = toCity.Longitude }, hotelsNeeded + 2).OrderBy(h => Guid.NewGuid()).ToList();

            var beginHotelDay = 1;
            while (beginHotelDay < form.NumDays)
            {
                var hotel = hotels.First();
                hotels.Remove(hotel);
                var daysInThisHotel = Math.Min(r.Next(minimumNightStay, minimumNightStay + 2), form.NumDays - beginHotelDay);
                trip.Activities.Add(new HotelActivity { BeginDay = beginHotelDay, EndDay = beginHotelDay + daysInThisHotel, Hotel = hotel });
                beginHotelDay += daysInThisHotel;
            }

            // Transportation home
            trip.Activities.Add(new TransportationActivity { BeginDay = form.NumDays, EndDay = form.NumDays, TransportationType = fly, FromCity = toCity, ToCity = fromCity });
            return trip;
        }

        public TripSettingsForm GetSettingsForm(Trip trip)
        {
            var form = new TripSettingsForm
                           {
                               Name = trip.Name,
                               BeginDate = trip.BeginDate.HasValue ? trip.BeginDate.Value.ToShortDateString() : string.Empty
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