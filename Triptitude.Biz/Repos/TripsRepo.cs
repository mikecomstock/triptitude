﻿using System;
using System.Collections.Generic;
using System.Linq;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TripsRepo : Repo<Trip>
    {
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
            int minimumNightStay = form.NumberOfDays < 7 ? 3 : form.NumberOfDays < 14 ? 5 : form.NumberOfDays < 30 ? 7 : 10;
            int hotelsNeeded = (int)Math.Ceiling((decimal)form.NumberOfDays / minimumNightStay);

            var transportationTypesRepo = new TransportationTypesRepo();
            var fly = transportationTypesRepo.Find("Fly");

            var citiesRepo = new CitiesRepo();
            var fromCity = citiesRepo.Find(4930956);
            var toCity = citiesRepo.Find(form.DestinationId.Value);

            Trip trip = new Trip { Name = "My trip to " + toCity.ShortName, User = currentUser, Created_On = DateTime.UtcNow, Activities = new List<Activity>() };

            // Transportation to destination
            trip.Activities.Add(new TransportationActivity { BeginDay = 1, EndDay = 1, TransportationType = fly, FromCity = fromCity, ToCity = toCity });

            // Hotels for each day
            var hotelsRepo = new HotelsRepo();
            var hotels = hotelsRepo.IncrementalSearch(new HotelSearchForm { Latitude = toCity.Latitude, Longitude = toCity.Longitude }, hotelsNeeded + 2).OrderBy(h => Guid.NewGuid()).ToList();

            var beginHotelDay = 1;
            while (beginHotelDay < form.NumberOfDays)
            {
                var hotel = hotels.First();
                hotels.Remove(hotel);
                var daysInThisHotel = Math.Min(r.Next(minimumNightStay, minimumNightStay + 2), form.NumberOfDays - beginHotelDay);
                trip.Activities.Add(new HotelActivity { BeginDay = beginHotelDay, EndDay = beginHotelDay + daysInThisHotel, Hotel = hotel });
                beginHotelDay += daysInThisHotel;
            }

            // Transportation home
            trip.Activities.Add(new TransportationActivity { BeginDay = form.NumberOfDays, EndDay = form.NumberOfDays, TransportationType = fly, FromCity = toCity, ToCity = fromCity });
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