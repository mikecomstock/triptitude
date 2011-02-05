using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Services
{
    public class AuthService
    {
        public User Authenticate(string email, string password)
        {
            UsersRepo usersRepo = new UsersRepo();
            User user = usersRepo.FindByEmailPassword(email, password);
            return user;
        }
    }
}
