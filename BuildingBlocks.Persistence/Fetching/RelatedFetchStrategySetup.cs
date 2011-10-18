using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BuildingBlocks.Persistence.Fetching
{
    class RelatedFetchStrategySetup<T> : AbstractFetchStrategySetup<T>, IRelatedFetchStrategySetup<T>
    {
        public RelatedFetchStrategySetup(FetchCollection relatedFetches)
            : base(relatedFetches)
        {
        }

        public IRelatedFetchStrategySetup<TProp> ThenFetch<TProp>(Expression<Func<T, TProp>> relatedObjectSelector)
        {
            return Fetch(relatedObjectSelector);
        }

        public IRelatedFetchStrategySetup<TProp> ThenFetchMany<TProp>(Expression<Func<T, IEnumerable<TProp>>> relatedObjectSelector)
        {
            return FetchMany(relatedObjectSelector);
        }

        public void ConcatWith(IFetchStrategy<T> other)
        {
            var fetches = ((AbstractFetchStrategySetup<T>) other).Fetches;
            foreach (var fetch in fetches)
            {
                var existsFetch = _fetches.FindFetch(fetch.Member);
                if (existsFetch == null)
                {
                    _fetches.Add(fetch);
                }
                else
                {
                    existsFetch.CopyRelatedFetchesFrom(fetch);    
                }
            }
        }
    }
}