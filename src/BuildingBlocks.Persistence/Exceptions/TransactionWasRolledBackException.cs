namespace BuildingBlocks.Persistence.Exceptions
{
    public class TransactionWasRolledBackException : PersistenceException
    {
        public TransactionWasRolledBackException()
            : base("Transaction was rolled back")
        {
        }
    }
}
