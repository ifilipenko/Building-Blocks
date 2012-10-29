namespace BuildingBlocks.Store
{
    public interface ITransaction
    {
        bool IsRolledBack { get; }
        void SumbitChanges();
        void Rollback();
    }
}