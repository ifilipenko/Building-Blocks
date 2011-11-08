namespace BuildingBlocks.Persistence.Exceptions
{
    public class EqualityMembersShouldBeOverridenException : PersistenceException
    {
        private const string MessageText = "Equality members should be overriden, because {0}";

        public EqualityMembersShouldBeOverridenException(string reason) 
            : base(string.Format(MessageText, reason))
        {
        }
    }
}