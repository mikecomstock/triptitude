using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TagsRepo : Repo<Tag>
    {
        public Tag FindOrInitializeByName(string name)
        {
            name = name.Trim();
            name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
            Tag tag = FindAll().FirstOrDefault(t => t.Name == name);

            if (tag == null)
            {
                tag = new Tag { Name = name };
                Add(tag);
            }

            return tag;
        }

        public IEnumerable<Tag> FindOrInitializeAll(string tagString)
        {
            var tokens = tagString.Split(',');
            return tokens.Select(FindOrInitializeByName);
        }
    }
}