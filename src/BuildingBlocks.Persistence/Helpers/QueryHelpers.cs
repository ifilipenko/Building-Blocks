using System.Linq;
using BuildingBlocks.Persistence.Fetching;

namespace BuildingBlocks.Persistence.Helpers
{
    public static class QueryHelpers
    {
        public static IQueryable<T> FetchBy<T>(this IQueryable<T> queryable, IFetchStrategy<T> fetchStrategy)
        {
            return fetchStrategy.ApplyTo(queryable);
        }
    }
}