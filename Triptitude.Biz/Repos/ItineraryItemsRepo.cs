using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

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

        public ItineraryItem AddWebsiteToTrip(Website website, Trip trip)
        {
            ItineraryItem itineraryItem = new ItineraryItem
            {
                Trip = trip,
                Website = website
            };

            trip.Itinerary.Add(itineraryItem);
            Save();

            return itineraryItem;
        }

        public ItineraryItem AddBaseItemToTrip(BaseItem baseItem, Trip trip)
        {
            ItineraryItem itineraryItem = new ItineraryItem
                                              {
                                                  Trip = trip,
                                                  BaseItem = baseItem
                                              };

            trip.Itinerary.Add(itineraryItem);
            Save();

            return itineraryItem;
        }
    }
}