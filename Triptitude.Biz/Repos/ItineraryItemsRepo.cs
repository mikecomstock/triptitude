using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Services;

namespace Triptitude.Biz.Repos
{
    public class ItineraryItemsRepo : Repo<ItineraryItem>
    {
        public ItineraryItemSettings GetSettings(ItineraryItem itineraryItem)
        {
            var itinerarySettings = new ItineraryItemSettings
                                        {
                                            BeginDay = itineraryItem.BeginDay,
                                            EndDay = itineraryItem.EndDay
                                        };
            return itinerarySettings;
        }

        public void Save(ItineraryItem itineraryItem, ItineraryItemSettings settings)
        {
            itineraryItem.BeginDay = settings.BeginDay;
            itineraryItem.EndDay = settings.EndDay;
            Save();
        }

        // TODO: move this to a service class instead
        public ItineraryItem Save(WebsiteForm form)
        {
            ItineraryItem itineraryItem;

            if (form.ItineraryItemId.HasValue)
            {
                itineraryItem = Find(form.ItineraryItemId.Value);
            }
            else
            {
                itineraryItem = new ItineraryItem();
                Add(itineraryItem);
            }

            Trip trip = new TripsRepo().Find(form.TripId);
            itineraryItem.Trip = trip;

            // Use the existing site if it already exists. Otherwise add & thumbnail it.
            WebsitesRepo websitesRepo = new WebsitesRepo();
            itineraryItem.Website = websitesRepo.FindByUrl(form.Url) ?? new WebsiteService().AddWebsite(form.Url);

            itineraryItem.BeginDay = form.BeginDay;
            itineraryItem.EndDay = form.EndDay;

            Save();

            return itineraryItem;
        }

        // TODO: move this to a service class instead
        public ItineraryItem AddHotelToTrip(Hotel hotel, Trip trip)
        {
            ItineraryItem itineraryItem = new ItineraryItem
                                              {
                                                  Trip = trip,
                                                  Hotel = hotel
                                              };

            trip.Itinerary.Add(itineraryItem);
            Save();

            return itineraryItem;
        }
    }
}