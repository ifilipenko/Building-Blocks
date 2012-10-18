namespace BuildingBlocks.Query
{
    public interface IQueryFactory
    {
        IQuery<TCriteria, TResult> Create<TCriteria, TResult>();
        IQuery<TResult> Create<TResult>();
    }
}