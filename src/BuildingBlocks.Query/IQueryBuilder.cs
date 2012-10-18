namespace BuildingBlocks.Query
{
    public interface IQueryBuilder
    {
        IQueryFor<TResult> For<TResult>();
    }
}