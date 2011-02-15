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
            _db.SaveChanges();
        }
    }
}