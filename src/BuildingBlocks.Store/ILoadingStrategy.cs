using System;
using System.Linq.Expressions;

namespace BuildingBlocks.Store
{
    public interface ILoadingStrategy<T>
    {
        ILoadingStrategy<T> Include(Expression<Func<T, object>> property);
    }
}