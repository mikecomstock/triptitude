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
        public ItineraryItem AddWebsiteToTrip(WebsiteForm form, Trip trip)
        {
            Website website = new WebsiteService().AddWebsite(form.Url);

            ItineraryItem itineraryItem = new ItineraryItem
            {
                Trip = trip,
                Website = website,
                BeginDay = form.BeginDay,
                EndDay = form.EndDay
            };

            trip.Itinerary.Add(itineraryItem);
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