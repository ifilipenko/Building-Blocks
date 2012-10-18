using BuildingBlocks.Common;

namespace BuildingBlocks.Query
{
    public class QueryFactory : IQueryFactory
    {
        private readonly IIocContainer _container;

        public QueryFactory(IIocContainer container)
        {
            _container = container;
        }

        public IQuery<TCriteria, TResult> Create<TCriteria, TResult>()
        {
            return _container.Resolve<IQuery<TCriteria, TResult>>();
        }

        public IQuery<TResult> Create<TResult>()
        {
            return _container.Resolve<IQuery<TResult>>();
        }
    }
}