using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class ActivitiesRepo : Repo<Activity>
    {
        private static void SetBaseProperties(Activity activity, ActivityForm form)
        {
            activity.Trip = new TripsRepo().Find(form.TripId);
            activity.BeginDay = form.BeginDay.Value;
            activity.EndDay = form.EndDay.Value;
        }

        public Activity Save(TransportationActivityForm form)
        {
            TransportationActivity activity;

            if (form.ActivityId.HasValue)
            {
                activity = (TransportationActivity)Find(form.ActivityId.Value);
            }
            else
            {
                activity = new TransportationActivity();
                Add(activity);
            }

            SetBaseProperties(activity, form);

            var type = new TransportationTypesRepo().Find(form.TransportationTypeId);
            activity.TransportationType = type;

            var citiesRepo = new CitiesRepo();
            activity.FromCity = citiesRepo.Find(form.FromCityId);
            activity.ToCity = citiesRepo.Find(form.ToCityId);

            Save();

            return activity;
        }

        public Activity Save(HotelActivityForm form)
        {
            HotelActivity activity;

            if (form.ActivityId.HasValue)
            {
                activity = (HotelActivity)Find(form.ActivityId.Value);
            }
            else
            {
                activity = new HotelActivity();
                Add(activity);
            }

            SetBaseProperties(activity, form);

            HotelsRepo hotelsRepo = new HotelsRepo();
            activity.Hotel = hotelsRepo.Find(form.HotelId);

            Save();

            return activity;
        }

        public Activity Save(WebsiteActivityForm form)
        {
            WebsiteActivity activity;

            if (form.ActivityId.HasValue)
            {
                activity = (WebsiteActivity)Find(form.ActivityId.Value);
            }
            else
            {
                activity = new WebsiteActivity();
                Add(activity);
            }

            SetBaseProperties(activity, form);

            activity.URL = form.Url;// .WebsiteActivity = websitesRepo.FindByUrl(form.Url) ?? new WebsiteService().AddWebsite(form.Url);
            activity.Title = "TODO: set this title";
            
            Save();

            return activity;
        }

        public Activity Save(TagActivityForm form)
        {
            TagActivity activity;

            if (form.ActivityId.HasValue)
            {
                activity = (TagActivity)Find(form.ActivityId.Value);
            }
            else
            {
                activity = new TagActivity();
                Add(activity);
            }

            SetBaseProperties(activity, form);

            Tag tag = new TagsRepo().FindOrCreateByName(form.TagName);
            activity.Tag = tag;

            City city = new CitiesRepo().Find(form.CityId);
            activity.City = city;

            Save();

            return activity;
        }
    }
}