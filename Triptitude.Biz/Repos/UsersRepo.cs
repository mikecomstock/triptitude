using System;
using System.Linq;
using DevOne.Security.Cryptography.BCrypt;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class UsersRepo : Repo<User>
    {
        public User FindByEmailPassword(string email, string password)
        {
            User user = FindAll().FirstOrDefault(u => u.Email == email);
            bool hashesMatch = BCryptHelper.CheckPassword(password, user.HashedPassword);

            if (hashesMatch) return user;
            else return null;
        }

        public User FindOrInitialize(string anonymousId)
        {
            if (string.IsNullOrWhiteSpace(anonymousId)) throw new ArgumentNullException("anonymousId");

            User user = FindAll().FirstOrDefault(u => u.AnonymousId == anonymousId);
            if (user == null)
            {
                user = new User { AnonymousId = anonymousId };
            }
            return user;
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

        public void MigrateAnonymousUser(string anonymousID, int userId)
        {
            User anonymousUser = FindAll().FirstOrDefault(u => u.AnonymousId == anonymousID);
            User registeredUser = Find(userId);
            if (anonymousUser != null)
            {
                anonymousUser.Trips.ToList().ForEach(t => t.User = registeredUser);
                //TODO: do this for every table that has a user_id
            }

            Save();
        }
    }
}