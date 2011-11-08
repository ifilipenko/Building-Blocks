using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BuildingBlocks.Persistence.Fetching
{
    public class AbstractFetchStrategySetup<T> : IFetchStrategySetup<T>
    {
        protected readonly FetchCollection _fetches;

        public AbstractFetchStrategySetup()
        {
            _fetches = new FetchCollection();
        }

        internal AbstractFetchStrategySetup(FetchCollection fetches)
        {
            _fetches = fetches;
        }

        public IEnumerable<Fetch> Fetches
        {
            get { return _fetches; }
        }

        public IRelatedFetchStrategySetup<TProp> Fetch<TProp>(Expression<Func<T, TProp>> relatedObjectSelector)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(TProp)))
            {
                throw new InvalidOperationException("For collection fetching need use \"FetchMany\" method");
            }

            var fetch = _fetches.EnsureFetch(relatedObjectSelector);
            return new RelatedFetchStrategySetup<TProp>(fetch.RelatedFetchesCollection);
        }

        public IRelatedFetchStrategySetup<TProp> FetchMany<TProp>(Expression<Func<T, IEnumerable<TProp>>> relatedObjectSelector)
        {
            var fetch = _fetches.EnsureFetch(relatedObjectSelector);
            return new RelatedFetchStrategySetup<TProp>(fetch.RelatedFetchesCollection);
        }
    }
}