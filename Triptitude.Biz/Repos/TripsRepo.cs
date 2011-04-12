using System;
using System.Collections.Generic;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TripsRepo : Repo<Trip>
    {
        public Trip Save(TripCreate form, int savedByUserId)
        {
            Trip trip = new Trip
                            {
                                Name = form.Name,
                                Created_By = savedByUserId,
                                Created_On = DateTime.UtcNow
                            };
            _db.Trips.Add(trip);

            User user = _db.Users.Find(savedByUserId);
            trip.Users = new List<User> { user };

            Save();
            return trip;
        }

        public TripSettingsForm GetSettingsForm(Trip trip)
        {
            var form = new TripSettingsForm()
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