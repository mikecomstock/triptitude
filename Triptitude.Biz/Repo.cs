using System;
using System.Collections.Generic;
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

        public IEnumerable<T> Sql(string sql, params object[] parameters)
        {
            return DbProvider._db.Set<T>().SqlQuery(sql, parameters);
        }

        public void Add(T entity)
        {
            DbProvider._db.Set<T>().Add(entity);
        }

        public virtual void Delete(T entity)
        {
            DbProvider._db.Set<T>().Remove(entity);
        }

        public virtual void Delete<F>(F entity) where F : class
        {
            DbProvider._db.Set<F>().Remove(entity);
        }

        public void Save()
        {
            DbProvider.Save();
        }
    }

    public class Repo
    {
        protected Db _db { get { return DbProvider._db; } }
        public void ExecuteSql(string sql, params object[] paramegers)
        {
            DbProvider._db.Database.ExecuteSqlCommand(sql, paramegers);
        }
    }

    // Is bad code less bad if you write a comment about how bad it is? no. MC
    internal class DbProvider
    {
        [ThreadStatic]
        internal static Db _consoleDb;

        internal static Db _db
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    if (_consoleDb == null)
                        _consoleDb = new Db();
                    return _consoleDb;
                }
                else
                {
                    if (HttpContext.Current.Items["db"] == null)
                        HttpContext.Current.Items["db"] = new Db();
                    return (Db)HttpContext.Current.Items["db"];
                }
            }
        }

        internal static void Save()
        {
            _db.SaveChanges();
        }
    }
}