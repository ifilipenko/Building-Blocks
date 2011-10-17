namespace BuildingBlocks.Configuration.Automapper
{
    public interface IAutomapperMaps<T>
    {
        void CreateMaps(IAutomapperMapFactory<T> mapFactory);
    }
}