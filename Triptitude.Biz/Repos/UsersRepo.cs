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
    }
}