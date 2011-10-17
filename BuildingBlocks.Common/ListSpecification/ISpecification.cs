namespace BuildingBlocks.Common.ListSpecification
{
    public interface ISpecification<in T>
    {
        bool Matches(T value);
    }
}