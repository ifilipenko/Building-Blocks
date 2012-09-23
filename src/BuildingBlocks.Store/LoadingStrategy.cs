using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BuildingBlocks.Store
{
    public class LoadingStrategy
    {
        public static ILoadingStrategy<T> Of<T>()
        {
            return new LoadingStrategy<T>();
        }
    }

    public class LoadingStrategy<T> : ILoadingStrategy<T>
    {
        private readonly Queue<Expression<Func<T, object>>> _propertiesQueue = new Queue<Expression<Func<T, object>>>();

        public bool IsEmpty
        {
            get { return _propertiesQueue.Count > 0; }
        }

        public ILoadingStrategy<T> Include(Expression<Func<T, object>> property)
        {
            _propertiesQueue.Enqueue(property);
            return this;
        }

        public Expression<Func<T, object>> Dequeue()
        {
            return _propertiesQueue.Dequeue();
        }
    }
}