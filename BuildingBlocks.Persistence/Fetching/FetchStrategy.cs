using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BuildingBlocks.Persistence.Fetching
{
    public class FetchStrategy<T> : AbstractFetchStrategySetup<T>, IFetchStrategy<T>
    {
        public void ConcatWithOther<TProp>(
            Expression<Func<T, TProp>> relatedObjectSelector,
            IFetchStrategy<TProp> other)
        {
            Fetch(relatedObjectSelector).ConcatWith(other);
        }

        public void ConcatWithOther<TProp>(
            Expression<Func<T, IEnumerable<TProp>>> relatedObjectSelector,
            IFetchStrategy<TProp> other)
        {
            FetchMany(relatedObjectSelector).ConcatWith(other);
        }

        public IQueryable<T> ApplyTo(IQueryable<T> queryable)
        {
            foreach (var fetch in _fetches)
            {
                queryable = fetch.ApplyFetch(queryable);
            }
            return queryable;
        }
    }
}