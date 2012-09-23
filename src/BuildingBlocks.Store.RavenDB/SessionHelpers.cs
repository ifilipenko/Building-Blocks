using System;
using System.Linq;
using Raven.Client.Linq;

namespace BuildingBlocks.Store.RavenDB
{
    public static class SessionHelpers
    {
        public static IQueryable<T> WaitForNonStaleResultsAsOfLastWrite<T>(this IQueryable<T> queryable)
        {
            var ravenQueryable = queryable as IRavenQueryable<T>;
            if (ravenQueryable != null)
            {
                return ravenQueryable.Customize(customization => customization.WaitForNonStaleResultsAsOfLastWrite(TimeSpan.FromSeconds(5)));
            }
            return queryable;
        }
    }
}