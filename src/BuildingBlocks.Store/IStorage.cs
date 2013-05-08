namespace BuildingBlocks.Store
{
    public interface IStorage
    {
        void CheckConnection(int timeoutMilliseconds = 5000);
        IStorageSession OpenSession();
    }
}