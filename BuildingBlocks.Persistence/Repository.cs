using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace BuildingBlocks.Persistence
{
    public sealed class Repository : RepositoryBase
    {
        public long GetCount<T>(QueryOver<T> detachedQueryOver = null) 
            where T : class
        {
            var query = detachedQueryOver == null
                ? CurrentSession.QueryOver<T>()
                : detachedQueryOver.GetExecutableQueryOver(CurrentSession);
            return query.RowCountInt64();
        }

        public T GetByID<T>(object id)
            where T : class
        {
            return CurrentSession.Get<T>(id);
        }

        public IQuery Hql(string hql)
        {
            return CurrentSession.CreateQuery(hql);
        }

        public IQueryOver<T, T> QueryOver<T>()
            where T : class
        {
            return CurrentSession.QueryOver<T>();
        }

        public IQueryOver<T, T> QueryOver<T>(QueryOver<T> detachedQueryOver)
            where T : class
        {
            return detachedQueryOver.GetExecutableQueryOver(CurrentSession);
        }

        public IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias)
            where T : class
        {
            return CurrentSession.QueryOver(alias);
        }

        public IQueryable<T> Query<T>()
            where T : class
        {
            return CurrentSession.Query<T>();
        }

        public void SaveEntities<T>(IEnumerable<T> entities)
            where T : class
        {
            foreach (var entity in entities)
            {
                CurrentSession.SaveOrUpdate(entity);
            }
        }

        public void Save<T>(T entity)
            where T : class
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(string hql)
        {
            CurrentSession.Delete(hql);
        }

        public void Delete<T>(Expression<Func<T, bool>> condition) 
            where T : class
        {
            foreach (var entity in Query<T>().Where(condition))
            {
                CurrentSession.Delete(entity);
            }
        }

        public void Delete<T>(T entity) 
            where T : class
        {
            CurrentSession.Delete(entity);
        }

        public void Delete<T>(IEnumerable<T> entities)
            where T : class
        {
            foreach (var entity in entities)
            {
                CurrentSession.Delete(entity);
            }
        }
    }
}