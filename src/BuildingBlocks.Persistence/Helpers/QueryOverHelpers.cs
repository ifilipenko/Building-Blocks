using System;
using System.Linq;
using System.Linq.Expressions;
using BuildingBlocks.Common.ListSpecification;
using BuildingBlocks.Persistence.Specification;
using CuttingEdge.Conditions;
using NHibernate;
using NHibernate.Criterion;

namespace BuildingBlocks.Persistence.Helpers
{
    public static class QueryOverHelpers
    {
        public static QueryOver<TRoot, TSubtype> WhereSatisfiedBy<TRoot, TSubtype>(
            this QueryOver<TRoot, TSubtype> query, 
            IListSpecification<TRoot> listSpecification)
        {
            Condition.Requires(listSpecification, "listSpecification").IsNotNull();
            Condition.Requires(query, "query").IsNotNull();

            var listSpecificationQueryOverBuilder = new ListSpecificationQueryOverBuilder(query);
            listSpecificationQueryOverBuilder.ApplyFilter(listSpecification);

            return query;
        }

        public static QueryOver<TRoot, TSubtype> OrderBy<TRoot, TSubtype>(
            this QueryOver<TRoot, TSubtype> query,
            IListSpecification<TRoot> listSpecification)
        {
            Condition.Requires(listSpecification, "listSpecification").IsNotNull();
            Condition.Requires(query, "query").IsNotNull();

            var listSpecificationQueryOverBuilder = new ListSpecificationQueryOverBuilder(query);
            listSpecificationQueryOverBuilder.ApplyOrders(listSpecification);

            return query;
        }

        public static IListSpecificationQueryOverExecution<TRoot> WithListSpecification<TRoot, TSubtype>(
            this IQueryOver<TRoot, TSubtype> query,
            IListSpecification<TRoot> listSpecification)
        {
            return new ListSpecificationQueryOverExecution<TRoot, TSubtype>(query, listSpecification);
        }

        public static IQueryOver<T, TSub> EagerLoad<T, TSub>(
            this IQueryOver<T, TSub> query, 
            params Expression<Func<T, object>>[] eagerProperties)
        {
            if (eagerProperties == null)
                return query;
            foreach (var eagerProperty in eagerProperties)
            {
                if (eagerProperty != null)
                    query = query.Fetch(eagerProperty).Eager;
            }
            return query;
        }

        public static QueryOver<TRoot, TSubtype> WithDisjunction<TRoot, TSubtype>(
            this QueryOver<TRoot, TSubtype> queryOver,
            params AbstractCriterion[] criterionList)
        {
            var disjunction = new Disjunction();
            foreach (var criterion in criterionList ?? Enumerable.Empty<AbstractCriterion>())
            {
                disjunction.Add(criterion);
            }
            return queryOver.Where(disjunction);
        }
    }
}
