namespace BuildingBlocks.Common
{
    public interface IIocContainer
    {
        T Resolve<T>();
    }
}