using System.Globalization;
using System.Linq;
using Triptitude.Biz.Forms;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class ItemRepo : Repo<Item>
    {
        public Item FindOrInitialize(string name)
        {
            var item = FindAll().FirstOrDefault(i => i.Name == name);
            if (item == null)
            {
                item = new Item { Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name) };
            }
            return item;
        }
    }

    public class ItemTagRepo : Repo<ItemTag>
    {
        public ItemTag FindOrInitialize(Item item, Tag tag)
        {
            var query = FindAll().Where(it => it.Item.Id == item.Id);
            query = tag == null ? query.Where(it => it.Tag == null) : query.Where(it => it.Tag.Id == tag.Id);
            var itemTag = query.FirstOrDefault();
            if (itemTag == null)
            {
                itemTag = new ItemTag { Item = item, Tag = tag, ShowInSite = true };
                Add(itemTag);
            }
            return itemTag;
        }

        public IQueryable<ItemTag> MostPopular(int take, Tag tag = null)
        {
            var possibilities = FindAll();

            if (tag != null)
                possibilities = possibilities.Where(it => it.Tag.Id == tag.Id);

            var itemTags = (from it in possibilities
                            orderby it.PackingListItems.Count() descending, it.Item.Name
                            where it.ShowInSite
                            select it).Take(take);
            return itemTags;
        }
    }

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

            if (string.IsNullOrWhiteSpace(form.Name))
            {
                Delete(packingListItem);
                Save();
                return null;
            }

            // once the trip is set, don't allow it to change
            if (packingListItem.Trip == null) packingListItem.Trip = new TripsRepo().Find(form.TripId);
            packingListItem.Note = form.Note;
            packingListItem.Visibility_Id = form.Visibility_Id;
            packingListItem.TagString = form.TagString;

            if (string.IsNullOrWhiteSpace(form.GoogReference))
            {
                var tmp = packingListItem.Place; // Because Entity Framework is stupid...
                packingListItem.Place = null;
            }
            else
            {
                var placesRepo = new PlacesRepo();
                packingListItem.Place = placesRepo.FindOrInitializeByGoogReference(form.GoogId, form.GoogReference);
            }

            var item = new ItemRepo().FindOrInitialize(form.Name);
            Tag tag = null;
            if (!string.IsNullOrWhiteSpace(form.TagString))
            {
                tag = new TagsRepo().FindOrInitializeAll(form.TagString).First();
            }

            var itemTagRepo = new ItemTagRepo();
            var itemTag = itemTagRepo.FindOrInitialize(item, tag);
            packingListItem.ItemTag = itemTag;

            Save();

            return packingListItem;
        }

        public PackingItemForm GetForm(int id)
        {
            PackingListItem packingListItem = Find(id);

            PackingItemForm form = new PackingItemForm
                                    {
                                        Name = packingListItem.ItemTag.Item.Name,
                                        Note = packingListItem.Note,
                                        Visibility_Id = packingListItem.Visibility_Id,
                                        TripId = packingListItem.Trip.Id,
                                        GoogId = packingListItem.Place == null ? null : packingListItem.Place.GoogId,
                                        GoogReference = packingListItem.Place == null ? null : packingListItem.Place.GoogReference,
                                        GoogName = packingListItem.Place == null ? null : packingListItem.Place.Name,
                                        PackingItemId = packingListItem.Id,
                                        TagString = packingListItem.TagString
                                    };
            return form;
        }
    }
}