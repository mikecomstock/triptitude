using System.Linq;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class PackingListItemsRepo : Repo<PackingListItem>
    {
        public PackingListItem Save(PackingItemForm form)
        {
            PackingListItem packingListItem;

            if (form.PackingItemId.HasValue)
            {
                packingListItem = Find(form.PackingItemId.Value);
            }
            else
            {
                packingListItem = new PackingListItem();
                Add(packingListItem);
            }

            // once the trip is set, don't allow the it to change
            if (packingListItem.Trip == null)
                packingListItem.Trip = new TripsRepo().Find(form.TripId);

            packingListItem.Activity = form.ActivityId.HasValue ? new ActivitiesRepo().Find(form.ActivityId.Value) : null;
            packingListItem.Name = form.Name;
            packingListItem.Note = form.Note;
            packingListItem.Public = form.Public;

            packingListItem.TagString = form.TagString;
            if (packingListItem.Tags != null) packingListItem.Tags.Clear();
            if (!string.IsNullOrWhiteSpace(form.TagString))
            {
                packingListItem.Tags = new TagsRepo().FindOrInitializeAll(form.TagString).ToList();
            }

            Save();

            return packingListItem;
        }

        public PackingItemForm GetForm(int id)
        {
            PackingListItem packingListItem = Find(id);

            PackingItemForm form = new PackingItemForm
                                    {
                                        ActivityId = packingListItem.Activity == null ? (int?)null : packingListItem.Activity.Id,
                                        Name = packingListItem.Name,
                                        Note = packingListItem.Note,
                                        Public = packingListItem.Public,
                                        TripId = packingListItem.Trip.Id,
                                        PackingItemId = packingListItem.Id,
                                        TagString = packingListItem.TagString
                                    };
            return form;
        }
    }
}