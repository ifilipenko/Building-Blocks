using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using BuildingBlocks.Common.Utils;

namespace BuildingBlocks.Persistence.Fetching
{
    public class FetchCollection : List<Fetch>
    {
        public Fetch FindFetch(LambdaExpression expression)
        {
            return FindFetch(expression.GetMemberInfo());
        }

        public Fetch FindFetch(MemberInfo member)
        {
            return Find(f => f.Member == member);
        }

        public Fetch EnsureFetch(LambdaExpression expression)
        {
            var fetch = FindFetch(expression);
            if (fetch == null)
            {
                fetch = new Fetch(expression);
                Add(fetch);
            }
            return fetch;
        }

        public void MergeWithFetchesFrom(IEnumerable<Fetch> other)
        {
            foreach (var otherRelatedFetch in other)
            {
                var existsFetch = FindFetch(otherRelatedFetch.Member);
                if (existsFetch == null)
                {
                    Add(otherRelatedFetch);
                }
                else
                {
                    existsFetch.CopyRelatedFetchesFrom(otherRelatedFetch);
                }
            }
        }
    }
}