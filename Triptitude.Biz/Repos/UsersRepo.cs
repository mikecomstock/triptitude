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

        public void SetDefaultTrip(User user, Trip trip)
        {
            user.DefaultTrip = trip;
            _db.SaveChanges();
        }
    }
}