using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class TagsRepo : Repo<Tag>
    {
        public Tag FindOrInitializeByName(string name)
        {
            name = name.Replace('_', '-');
            name = Regex.Replace(name, "[^a-zA-Z0-9-]+", "", RegexOptions.Compiled);

            if (string.IsNullOrWhiteSpace(name))
                return null;

            name = name.ToLower();
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
            var tokens = tagString.Split(' ');
            return tokens.Select(FindOrInitializeByName).Where(t => t != null);
        }
    }
}