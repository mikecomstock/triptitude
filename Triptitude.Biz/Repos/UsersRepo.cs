using System;
using System.Linq;
using DevOne.Security.Cryptography.BCrypt;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class UsersRepo : Repo<User>
    {
        public User FindByEmailAndPassword(string email, string password)
        {
            User user = FindAll().FirstOrDefault(u => u.Email == email);
            if (user == null) return null;

            bool hashesMatch = BCryptHelper.CheckPassword(password, user.HashedPassword);
            return hashesMatch ? user : null;
        }

        public User FindByEmail(string email)
        {
            User user = FindAll().FirstOrDefault(u => u.Email == email);
            return user;
        }

        public User FindByToken(Guid token)
        {
            User user = FindAll().FirstOrDefault(u => u.Guid == token);
            return user;
        }

        public User FindOrInitialize(string anonymousId)
        {
            if (string.IsNullOrWhiteSpace(anonymousId)) throw new ArgumentNullException("anonymousId");
            User user = FindAll().FirstOrDefault(u => u.AnonymousId == anonymousId) ?? new User { AnonymousId = anonymousId };
            return user;
        }
        
        public enum UserSaveAction
        {
            NoAction, NewUserCreated, EmailAlreadyTaken
        }

        public void Save(UserSettingsForm form, User user, out UserSaveAction userSaveAction)
        {
            var existingUser = FindAll().FirstOrDefault(u => u.Id != user.Id && u.Email == form.Email);
            bool emailAlreadyTaken = existingUser != null;

            // Handle UserSaveAction
            if (emailAlreadyTaken) userSaveAction = UserSaveAction.EmailAlreadyTaken;
            else if (string.IsNullOrWhiteSpace(user.Email)) userSaveAction = UserSaveAction.NewUserCreated;
            else userSaveAction = UserSaveAction.NoAction;

            // Set users properties
            if (userSaveAction != UserSaveAction.EmailAlreadyTaken)
            {
                user.FirstName = form.FirstName;
                user.LastName = form.LastName;
                // For new users only (don't allow changes, just to simplify code for now)
                user.Email = string.IsNullOrWhiteSpace(user.Email) ? form.Email : user.Email;

                if (!string.IsNullOrWhiteSpace(form.Password))
                {
                    string salt = BCryptHelper.GenerateSalt(10);
                    string hashedPassword = BCryptHelper.HashPassword(form.Password, salt);
                    user.HashedPassword = hashedPassword;
                }

                // The user is probably in the db already, since they
                // likely created a trip, but check just to make sure.
                if (user.Id == 0)
                    Add(user);

                Save();
            }
        }

        public void SetDefaultTrip(User user, Trip trip)
        {
            user.DefaultTrip = trip;
            _db.SaveChanges();
        }

        public void SetNewGuidIfNeeded(User user)
        {
            if (!user.Guid.HasValue || !user.GuidCreatedOnUtc.HasValue || (DateTime.UtcNow - user.GuidCreatedOnUtc.Value).TotalDays > 1)
            {
                user.Guid = Guid.NewGuid();
                user.GuidCreatedOnUtc = DateTime.UtcNow;
                Save();
            }
        }

        public void MigrateAnonymousUser(string anonymousID, int userId)
        {
            User anonymousUser = FindAll().FirstOrDefault(u => u.AnonymousId == anonymousID);
            User registeredUser = Find(userId);
            if (anonymousUser != null)
            {
                //TODO: do this for every table that has a user_id
                anonymousUser.UserTrips.ToList().ForEach(ut=>ut.User = registeredUser);

                if (anonymousUser.DefaultTrip != null) registeredUser.DefaultTrip = anonymousUser.DefaultTrip;
            }

            Save();
        }
    }
}