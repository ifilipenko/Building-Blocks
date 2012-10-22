using System.Collections.Generic;
using BuildingBlocks.Common;

namespace BuildingBlocks.Query
{
    public class QueryBuilder : IQueryBuilder
    {
        private readonly IQueryFactory _queryFactory;

        public QueryBuilder(IQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public IQueryFactory QueryFactory
        {
            get { return _queryFactory; }
        }

        public IQueryFor<TResult> For<TResult>()
        {
            return new QueryFor<TResult>(_queryFactory);
        }

        class QueryFor<TResult> : IQueryFor<TResult>
        {
            private readonly IQueryFactory _queryFactory;

            public QueryFor(IQueryFactory queryFactory)
            {
                _queryFactory = queryFactory;
            }

            public IQueryFor<TResult> With<TCriteria>(TCriteria criteria)
            {
                return new QueryWithCriteria<TCriteria, TResult>(criteria, _queryFactory);
            }

            public TResult SingleOrDefault()
            {
                return _queryFactory.Create<TResult>().Execute();
            }

            public IEnumerable<TResult> Enumerable()
            {
                return _queryFactory.Create<IEnumerable<TResult>>().Execute();
            }

            public Page<TResult> Page()
            {
                return _queryFactory.Create<Page<TResult>>().Execute();
            }
        }

        class QueryWithCriteria<TCriteria, TResult> : IQueryFor<TResult>
        {
            private readonly TCriteria _criteria;
            private readonly IQueryFactory _queryFactory;

            public QueryWithCriteria(TCriteria criteria, IQueryFactory queryFactory)
            {
                _criteria = criteria;
                _queryFactory = queryFactory;
            }

            public IQueryFor<TResult> With<TOtherCriteria>(TOtherCriteria criteria)
            {
                return new QueryWithCriteria<TOtherCriteria, TResult>(criteria, _queryFactory);
            }

            public TResult SingleOrDefault()
            {
                return _queryFactory.Create<TCriteria, TResult>().Execute(_criteria);
            }

            public IEnumerable<TResult> Enumerable()
            {
                return _queryFactory.Create<TCriteria, IEnumerable<TResult>>().Execute(_criteria);
            }

            public Page<TResult> Page()
            {
                return _queryFactory.Create<TCriteria, Page<TResult>>().Execute(_criteria);
            }
        }
    }
}