using System.Linq;
using Triptitude.Biz.Models;

namespace Triptitude.Biz
{
    public class Repo<T> where T : class
    {
        protected readonly Db _db;

        public Repo()
        {
            _db = new Db();
        }

        public T Find(int id)
        {
            return _db.Set<T>().Find(id);
        }

        public IQueryable<T> FindAll()
        {
            return _db.Set<T>();
        }

        public void Add(T entity)
        {
            _db.Set<T>().Add(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}