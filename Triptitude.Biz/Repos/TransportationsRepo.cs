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

            var type = new TransportationTypesRepo().Find(form.TransportationTypeId);
            var trip = new TripsRepo().Find(form.TripId);
            var citiesRepo = new CitiesRepo();

            transportation.TransportationType = type;
            transportation.Trip = trip;
            transportation.ToCity = citiesRepo.Find(form.ToCityId);
            transportation.FromCity = citiesRepo.Find(form.FromCityId);
            transportation.BeginDay = form.BeginDay.Value;
            transportation.EndDay = form.EndDay.Value;
            Save();

            return transportation;
        }
    }
}