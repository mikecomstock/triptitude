using System.Web.Security;
using Triptitude.Biz.Models;

namespace Triptitude.Web.Helpers
{
    public static class AuthHelper
    {
        public static void SetAuthCookie(User user)
        {
            string userName = user.Id + "|" + user.Email;
            FormsAuthentication.SetAuthCookie(userName, true);
        }
    }
}