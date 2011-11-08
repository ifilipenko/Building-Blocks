using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BuildingBlocks.Persistence.Fetching
{
    public interface IFetchStrategySetup<T>
    {
        IRelatedFetchStrategySetup<TProp> Fetch<TProp>(Expression<Func<T, TProp>> relatedObjectSelector);
        IRelatedFetchStrategySetup<TProp> FetchMany<TProp>(Expression<Func<T, IEnumerable<TProp>>> relatedObjectSelector);
    }
}