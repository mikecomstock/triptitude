using Triptitude.Biz.Models;

namespace Triptitude.Biz.Services
{
    public class ItineraryService
    {
        public void AddWebsiteToTrip(Website website, Trip trip)
        {
            ItineraryItem itineraryItem = new ItineraryItem();
            itineraryItem.Trip = trip;
            itineraryItem.Website = website;

            trip.Itinerary.Add(itineraryItem);
            DbProvider.Save();
        }
    }
}