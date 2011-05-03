using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class SignupRepo : Repo<Signup>
    {
        public Signup Save(SignupForm form, string IP, string requestInfo)
        {
            Signup signup = new Signup
                                {
                                    Email = form.Email,
                                    TripName = form.TripName,
                                    GeoNameId = form.DestinationId,
                                    IP = IP,
                                    RequestInfo = requestInfo
                                };
            Add(signup);
            Save();
            return signup;
        }
    }
}