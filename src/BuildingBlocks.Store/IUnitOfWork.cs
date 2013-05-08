namespace BuildingBlocks.Store
{
    public interface IUnitOfWork
    {
        bool IsCancelled { get; }
        void SubmitChanges();
        void CancelChanges();
    }
}