using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Query
{
    public static class QueryBuilderHelpers
    {
        public static TResult FindById<TResult, TId>(this IQueryFor<TResult> queryFor, TId id)
        {
            return queryFor.With(new FindById<TId>(id)).SingleOrDefault();
        }

        public static IList<TResult> List<TResult>(this IQueryFor<TResult> queryFor)
        {
            var enumerable = queryFor.Enumerable();
            return enumerable is IList<TResult> 
                ? (IList<TResult>) enumerable 
                : enumerable.ToList();
        }
    }
}