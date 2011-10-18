using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BuildingBlocks.Persistence.Fetching
{
    public interface IRelatedFetchStrategySetup<T>
    {
        IRelatedFetchStrategySetup<TProp> ThenFetch<TProp>(Expression<Func<T, TProp>> relatedObjectSelector);
        IRelatedFetchStrategySetup<TProp> ThenFetchMany<TProp>(Expression<Func<T, IEnumerable<TProp>>> relatedObjectSelector);

        void ConcatWith(IFetchStrategy<T> other);
    }
}