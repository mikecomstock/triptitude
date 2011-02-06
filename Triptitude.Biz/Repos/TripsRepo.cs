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

            _db.SaveChanges();
            return trip;
        }
    }
}
