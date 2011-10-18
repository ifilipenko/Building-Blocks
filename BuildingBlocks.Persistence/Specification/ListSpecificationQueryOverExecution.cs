using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;
using BuildingBlocks.Common.ListSpecification;
using CuttingEdge.Conditions;
using NHibernate;
using NHibernate.Transform;

namespace BuildingBlocks.Persistence.Specification
{
    public class ListSpecificationQueryOverExecution<T, TSubtype> : IListSpecificationQueryOverExecution<T>
    {
        private readonly IQueryOver<T, TSubtype> _query;
        private readonly IListSpecification<T> _listSpecification;

        public ListSpecificationQueryOverExecution(IQueryOver<T, TSubtype> query, IListSpecification<T> listSpecification)
        {
            Condition.Requires(listSpecification, "listSpecification").IsNotNull();
            Condition.Requires(query, "query").IsNotNull();

            _query = query;
            _listSpecification = listSpecification;
        }

        #region Implementation of IListSpecificationQueryOverExecution<T,TSubtype>

        public IList<T> ToList(bool applyDistinctRoot)
        {
            ApplySpecification();
            return applyDistinctRoot
                       ? _query.TransformUsing(new DistinctRootEntityResultTransformer()).List()
                       : _query.List();
        }

        public IList<TResult> ToListOf<TResult>(bool applyDistinctRoot)
        {
            ApplySpecification();
            return applyDistinctRoot
                       ? _query.TransformUsing(new DistinctRootEntityResultTransformer()).List<TResult>()
                       : _query.List<TResult>();
        }

        public IPagedList<T> ToPagedList(bool applyDistinctRoot)
        {
            return (IPagedList<T>) GetPagedList(q => q.Future<T>(), r => r, applyDistinctRoot);
        }

        public IPagedList<TResult> ToPagedListOf<TResult>(bool applyDistinctRoot)
        {
            return (IPagedList<TResult>) GetPagedList(q => q.Future<TResult>(), r => r, applyDistinctRoot);
        }

        public IPagedList<TResult> ToMappedPagedList<TResult>(Func<T, TResult> map, bool applyDistinctRoot)
        {
            Condition.Requires(map, "map").IsNotNull();

            return (IPagedList<TResult>) GetPagedList(q => q.Future<T>(), map, applyDistinctRoot);
        }

        #endregion

        private void ApplySpecification()
        {
            var listSpecificationQueryOverBuilder = new ListSpecificationQueryOverBuilder(_query);
            listSpecificationQueryOverBuilder.ApplyFilter(_listSpecification);
            listSpecificationQueryOverBuilder.ApplyOrders(_listSpecification);
        }

        private IEnumerable GetPagedList<TLoadResult, TResult>(
            Func<IQueryOver<T, TSubtype>, IEnumerable<TLoadResult>> loadtItems, 
            Func<TLoadResult, TResult> mapItem,
            bool applyDistinctRoot)
        {
            Condition.Requires(_listSpecification.PagingEnabled, "PagingEnabled").IsTrue("Should paging enabled for query");

            ApplySpecification();

            if (applyDistinctRoot)
            {
                _query.TransformUsing(new DistinctRootEntityResultTransformer());
            }

            _query
                .Skip(_listSpecification.PageIndex.Value * _listSpecification.PageSize.Value)
                .Take(_listSpecification.PageSize.Value);
            var items = loadtItems(_query);

            _query.Skip(0).Take(0);
            var totalCount = _query.RowCount();

            var result = items.Select(mapItem).ToPagedList((int) _listSpecification.PageIndex,
                                                           (int) _listSpecification.PageSize,
                                                           totalCount);
            return result;
        }
    }
}