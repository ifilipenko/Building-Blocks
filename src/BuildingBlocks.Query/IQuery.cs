namespace BuildingBlocks.Query
{
    public interface IQuery<out TResult>
    {
        TResult Execute();
    }

    public interface IQuery<in TCriteria, out TResult>
    {
        TResult Execute(TCriteria criteria);
    }
}