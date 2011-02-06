using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class UsersRepo : Repo<User>
    {
        public User FindByEmailPassword(string email, string password)
        {
            User user = _db.Users.FirstOrDefault(u => u.Email == email);
            return user;
        }

        public void SetDefaultTrip(int userId, int tripId)
        {
            Find(userId).DefaultTrip = _db.Trips.Find(tripId);
            _db.SaveChanges();
        }
    }
}