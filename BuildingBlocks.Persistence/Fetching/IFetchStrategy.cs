using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BuildingBlocks.Persistence.Fetching
{
    public interface IFetchStrategy<T> : IFetchStrategySetup<T>
    {
        void ConcatWithOther<TProp>(
            Expression<Func<T, TProp>> relatedObjectSelector,
            IFetchStrategy<TProp> other);

        void ConcatWithOther<TProp>(
            Expression<Func<T, IEnumerable<TProp>>> relatedObjectSelector,
            IFetchStrategy<TProp> other);

        IQueryable<T> ApplyTo(IQueryable<T> queryable);
    }
}