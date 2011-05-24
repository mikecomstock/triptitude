using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;
using Triptitude.Biz.Services;

namespace Triptitude.Biz.Repos
{
    public class ActivitiesRepo: Repo<Activity>
    {
        public Activity Save(WebsiteForm form)
        {
            Activity itineraryItem;

            if (form.ItineraryItemId.HasValue)
            {
                itineraryItem = Find(form.ItineraryItemId.Value);
            }
            else
            {
                itineraryItem = null;// new Activity();
                Add(itineraryItem);
            }

            Trip trip = new TripsRepo().Find(form.TripId);
            itineraryItem.Trip = trip;

            // Use the existing site if it already exists. Otherwise add & thumbnail it.
            WebsitesRepo websitesRepo = new WebsitesRepo();
            itineraryItem.WebsiteActivity = websitesRepo.FindByUrl(form.Url) ?? new WebsiteService().AddWebsite(form.Url);

            itineraryItem.BeginDay = form.BeginDay.Value;
            itineraryItem.EndDay = form.EndDay.Value;

            Save();

            return itineraryItem;
        }

        public Activity Save(HotelForm form)
        {
            Activity itineraryItem;

            if (form.ItineraryItemId.HasValue)
            {
                itineraryItem = Find(form.ItineraryItemId.Value);
            }
            else
            {
                itineraryItem = null;// new Activity();
                Add(itineraryItem);
            }

            Trip trip = new TripsRepo().Find(form.TripId);
            itineraryItem.Trip = trip;

            HotelsRepo hotelsRepo = new HotelsRepo();
            //itineraryItem.LodgingActivity = hotelsRepo.Find(form.HotelId);

            itineraryItem.BeginDay = form.BeginDay.Value;
            itineraryItem.EndDay = form.EndDay.Value;

            Save();

            return itineraryItem;
        }

        public Activity Save(DestinationTagForm form)
        {
            Activity itineraryItem;

            if (form.ItineraryItemId.HasValue)
            {
                itineraryItem = Find(form.ItineraryItemId.Value);
            }
            else
            {
                itineraryItem = null;// new Activity();
                Add(itineraryItem);
            }

            Trip trip = new TripsRepo().Find(form.TripId);
            itineraryItem.Trip = trip;

            City city = new CitiesRepo().Find(form.DestinationId);
            Tag tag = new TagsRepo().FindOrCreateByName(form.TagName);

            itineraryItem.Tag = tag;
            itineraryItem.City = city;

            itineraryItem.BeginDay = form.BeginDay.Value;
            itineraryItem.EndDay = form.EndDay.Value;

            Save();

            return itineraryItem;
        }
    }
}