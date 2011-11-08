namespace BuildingBlocks.Persistence.Exceptions
{
    public class SessionScopeException : PersistenceException
    {
        public SessionScopeException(string message) 
            : base(message)
        {
        }
    }
}