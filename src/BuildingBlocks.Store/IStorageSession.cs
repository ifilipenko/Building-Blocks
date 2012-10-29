using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Store
{
    public interface IStorageSession : ITransaction, IDisposable
    {
        bool IsInitialized { get; }

        T GetById<T>(string id, ILoadingStrategy<T> loadingStrategy = null);

        T[] GetByIds<T>(params string[] ids);

        T[] GetByIds<T>(IEnumerable<string> ids, ILoadingStrategy<T> loadingStrategy = null);

        T GetById<T>(ValueType id, ILoadingStrategy<T> loadingStrategy = null);

        IQueryable<T> Query<T>(ILoadingStrategy<T> loadingStrategy = null);

        void Save(IEntity<string> entity);

        void Save<TId>(IEntity<TId> entity)
            where TId : struct;

        void Delete<T>(T entity);
    }
}