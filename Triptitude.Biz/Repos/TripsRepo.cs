using System;
using System.Linq;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TripsRepo : Repo<Trip>
    {
        public Trip Save(TripCreate form)
        {
            Trip trip = new Trip
                            {
                                Name = form.Name,
                                Created_By = 1,
                                Created_On = DateTime.UtcNow
                            };
            _db.Trips.Add(trip);
            _db.SaveChanges();
            return trip;
        }
    }

    public class UsersRepo : Repo<User>
    {
        public User FindByEmailPassword(string email, string password)
        {
            User user = _db.Users.FirstOrDefault(u => u.Email == email);
            return user;
        }
    }
}
