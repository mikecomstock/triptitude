using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TransportationsRepo : Repo<TransportationActivity>
    {
        public TransportationActivity Save(TransportationForm form)
        {
            TransportationActivity transportation;

            if (form.Id.HasValue)
            {
                transportation = Find(form.Id.Value);
            }
            else
            {
                transportation = new TransportationActivity();
                Add(transportation);
            }

            var type = new TransportationTypesRepo().Find(form.TransportationTypeId);
            var trip = new TripsRepo().Find(form.TripId);
            var citiesRepo = new CitiesRepo();

            transportation.TransportationType = type;
            //transportation.Trip = trip;
            transportation.ToCity = citiesRepo.Find(form.ToCityId);
            transportation.FromCity = citiesRepo.Find(form.FromCityId);
            Save();

            return transportation;
        }
    }
}