using System.Linq;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Forms
{
    public class TripSearchForm
    {
        public string TagString { get; set; }

        //public int RadiusInMiles { get; set; }

        public string GoogId { get; set; }
        public string GoogReference { get; set; }
        public string GoogName { get; set; } // for display only

        public int Take { get; set; }

        // For ActionResult binding
        public TripSearchForm()
        {
            Take = 10;
        }

        public TripSearchForm(Place place = null, Tag tag = null, int take = 10)
        {
            if (place != null)
            {
                GoogId = place.GoogId;
                GoogReference = place.GoogReference;
                GoogName = place.Name;
            }

            if (tag != null)
            {
                TagString = tag.Name;
            }

            Take = take;
        }
        
        private IQueryable<Trip> _Results;
        public IQueryable<Trip> Results
        {
            get
            {
                if (_Results == null)
                {
                    var trips = new TripsRepo().Search(this);
                    trips = trips.Where(t => t.ShowInSearch);
                    _Results = trips.Take(Take);
                }
                return _Results;
            }
        }
    }
}