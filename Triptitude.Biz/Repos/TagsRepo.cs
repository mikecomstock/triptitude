using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TagsRepo : Repo<Tag>
    {
        public Tag FindOrCreateByName(string name)
        {
            Tag tag = FindAll().FirstOrDefault(t => t.Name == name.Trim());

            if (tag == null)
            {
                tag = new Tag { Name = name.Trim() };
                Add(tag);
                Save();
            }

            return tag;
        }
    }
}