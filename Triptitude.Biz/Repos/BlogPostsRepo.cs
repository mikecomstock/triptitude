using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz.Repos
{
    public class BlogPostsRepo : Repo<BlogPost>
    {
        public IQueryable<BlogPost> FindByCategory(string category)
        {
            var posts = FindAll().Where(p => p.Category == category);
            return posts;
        }
    }
}