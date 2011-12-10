using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class ActivityPlacesRepo : Repo<ActivityPlace>
    {
        public ActivityPlace FindOrInitialize(Activity activity, int sortIndex, Place place)
        {
            var activityPlace = activity.ActivityPlaces.FirstOrDefault(ap => ap.SortIndex == sortIndex);
            if (activityPlace == null)
            {
                activityPlace = new ActivityPlace { SortIndex = sortIndex };
                activity.ActivityPlaces.Add(activityPlace);
            }
            activityPlace.Place = place;
            return activityPlace;
        }
    }
}