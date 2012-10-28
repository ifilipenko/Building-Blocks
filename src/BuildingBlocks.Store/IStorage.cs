namespace BuildingBlocks.Store
{
    public interface IStorage
    {
        IStorageSession OpenSesion();
    }
}