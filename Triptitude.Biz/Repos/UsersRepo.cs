using System;
using System.Linq;
using DevOne.Security.Cryptography.BCrypt;
using Triptitude.Biz.Exceptions;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class UsersRepo : Repo<User>
    {
        public User FindByEmailAndPassword(string email, string password)
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

        public UserSettingsForm GetSettingsForm(User user)
        {
            UserSettingsForm form = new UserSettingsForm
                                        {
                                            Email = user.Email
                                        };
            return form;
        }

        public void Save(UserSettingsForm form, User user)
        {
            var existingUser = FindAll().SingleOrDefault(u => u.Id != user.Id && u.Email == form.Email);
            if (existingUser != null)
                throw new EmailTakenException();

            user.Email = form.Email;

            if (!string.IsNullOrWhiteSpace(form.Password))
            {
                string salt = BCryptHelper.GenerateSalt(10);
                string hashedPassword = BCryptHelper.HashPassword(form.Password, salt);
                user.HashedPassword = hashedPassword;
            }

            if (user.Id == 0)
                Add(user);

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

                if (anonymousUser.DefaultTrip != null) registeredUser.DefaultTrip = anonymousUser.DefaultTrip;
            }

            Save();
        }
    }
}