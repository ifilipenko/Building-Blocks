namespace BuildingBlocks.Persistence.Exceptions
{
    public class EnumToEntityMapException : PersistenceException
    {
        public EnumToEntityMapException(string message) 
            : base(message)
        {
        }

        public EnumToEntityMapException()
        {
        }
    }
}