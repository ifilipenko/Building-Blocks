namespace BuildingBlocks.Persistence.Exceptions
{
    public class UnitOfWorkScopeNotOpenException : PersistenceException
    {
        public override string Message
        {
            get { return "Unit of work is not open"; }
        }
    }
}