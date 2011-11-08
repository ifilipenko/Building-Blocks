using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace BuildingBlocks.Persistence
{
    public abstract class Repository<T> : RepositoryBase
        where T : class
    {
        public long GetCount(QueryOver<T> detachedQueryOver = null)
        {
            var query = detachedQueryOver == null
                ? CurrentSession.QueryOver<T>()
                : detachedQueryOver.GetExecutableQueryOver(CurrentSession);
            return query.RowCountInt64();
        }

        public T GetByID(object id)
        {
            return CurrentSession.Get<T>(id);
        }

        public IQuery Hql(string hql)
        {
            return CurrentSession.CreateQuery(hql);
        }

        public IQueryOver<T, T> QueryOver()
        {
            return CurrentSession.QueryOver<T>();
        }

        public IQueryOver<T, T> QueryOver(QueryOver<T> detachedQueryOver)
        {
            return detachedQueryOver.GetExecutableQueryOver(CurrentSession);
        }

        public IQueryOver<T, T> QueryOver(Expression<Func<T>> alias)
        {
            return CurrentSession.QueryOver(alias);
        }

        public IQueryable<T> Query()
        {
            return CurrentSession.Query<T>();
        }

        public void SaveEntities(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                CurrentSession.SaveOrUpdate(entity);
            }
        }

        public void Save(T entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        protected void Save<TObject>(TObject entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(string hql)
        {
            CurrentSession.Delete(hql);
        }

        public void Delete(T entity)
        {
            CurrentSession.Delete(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                CurrentSession.Delete(entity);
            }
        }
    }
}