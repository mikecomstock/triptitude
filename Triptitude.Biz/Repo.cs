using System.Linq;
using System.Web;
using Triptitude.Biz.Models;

namespace Triptitude.Biz
{
    public class Repo<T> where T : class
    {
        protected Db _db { get { return DbProvider._db; } }

        public T Find(int id)
        {
            return DbProvider._db.Set<T>().Find(id);
        }

        public IQueryable<T> FindAll()
        {
            return DbProvider._db.Set<T>();
        }

        public void Add(T entity)
        {
            DbProvider._db.Set<T>().Add(entity);
        }
    }

    internal class DbProvider
    {
        internal static Db _db
        {
            get
            {
                // Is bad code less bad if you write a comment about how bad it is? no. MC
                if (HttpContext.Current.Request.RequestContext.HttpContext.Items["db"] == null)
                    HttpContext.Current.Request.RequestContext.HttpContext.Items["db"] = new Db();
                return (Db)HttpContext.Current.Request.RequestContext.HttpContext.Items["db"];
            }
        }

        public static void Save()
        {
            _db.SaveChanges();
        }
    }
}