namespace BuildingBlocks.Store
{
    public interface IUnitOfWork
    {
        bool IsCancelled { get; }
        void SumbitChanges();
        void CancelChanges();
    }
}