using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TransportationsRepo : Repo<Transportation>
    {
        public Transportation Save(TransportationForm form)
        {
            Transportation transportation;

            if (form.Id.HasValue)
            {
                transportation = Find(form.Id.Value);
            }
            else
            {
                transportation = new Transportation();
                Add(transportation);
            }

            var trip = new TripsRepo().Find(form.TripId);
            var citiesRepo = new CitiesRepo();

            transportation.Trip = trip;
            transportation.ToCity = citiesRepo.Find(form.ToCityId);
            transportation.FromCity = citiesRepo.Find(form.FromCityId);
            transportation.BeginDay = form.BeginDay;
            transportation.EndDay = form.EndDay;
            Save();

            return transportation;
        }
    }
}