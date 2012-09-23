using System;

namespace BuildingBlocks.Store
{
    public static class LoadingStrategyActionHelper
    {
        public static ILoadingStrategy<T> Setup<T>(this Action<ILoadingStrategy<T>> action)
        {
            if (action == null)
                return null;

            var loadingStrategy = new LoadingStrategy<T>();
            action(loadingStrategy);
            return loadingStrategy;
        }
    }
}