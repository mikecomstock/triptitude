using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Services;

namespace Triptitude.Biz.Repos
{
    public class ItineraryItemsRepo : Repo<ItineraryItem>
    {
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

            itineraryItem.BeginDay = form.BeginDay.Value;
            itineraryItem.EndDay = form.EndDay.Value;

            Save();

            return itineraryItem;
        }

        public ItineraryItem Save(HotelForm form)
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

            HotelsRepo hotelsRepo = new HotelsRepo();
            itineraryItem.Hotel = hotelsRepo.Find(form.HotelId);

            itineraryItem.BeginDay = form.BeginDay.Value;
            itineraryItem.EndDay = form.EndDay.Value;

            Save();

            return itineraryItem;
        }

        public ItineraryItem Save(DestinationTagForm form)
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

            City city = new CitiesRepo().Find(form.DestinationId);
            Tag tag = new TagsRepo().FindOrCreateByName(form.TagName);

            DestinationTagsRepo destinationTagsRepo = new DestinationTagsRepo();
            DestinationTag destinationTag = destinationTagsRepo.FindOrCreate(city, tag);

            itineraryItem.DestinationTag = destinationTag;

            itineraryItem.BeginDay = form.BeginDay.Value;
            itineraryItem.EndDay = form.EndDay.Value;

            Save();

            return itineraryItem;
        }
    }
}