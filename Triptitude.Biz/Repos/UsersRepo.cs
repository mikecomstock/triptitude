using System.Linq;
using DevOne.Security.Cryptography.BCrypt;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class UsersRepo : Repo<User>
    {
        public User FindByEmailPassword(string email, string password)
        {
            User user = _db.Users.FirstOrDefault(u => u.Email == email);
            bool hashesMatch = BCryptHelper.CheckPassword(password, user.HashedPassword);

            if (hashesMatch) return user;
            else return null;
        }

        public void SetPassword(User user, string password)
        {
            string salt = BCryptHelper.GenerateSalt(10);
            string hashedPassword = BCryptHelper.HashPassword(password, salt);
            user.HashedPassword = hashedPassword;
            Save();
        }

        public void SetDefaultTrip(User user, Trip trip)
        {
            user.DefaultTrip = trip;
            _db.SaveChanges();
        }
    }
}