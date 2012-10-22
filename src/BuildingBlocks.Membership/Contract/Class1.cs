namespace BuildingBlocks.Membership.Contract
{
    public static class RepositoryFactory
    {
        private static IRepositoryFactory _factoryImplementation;

        public static void Initialize(IRepositoryFactory factoryImplementation)
        {
            _factoryImplementation = factoryImplementation;
        }

        public static IRepositoryFactory Current
        {
            get { return _factoryImplementation; }
        }
    }

    public interface IRepositoryFactory
    {
        IUserRepository CreateUserRepository();
    }

    public interface IUserRepository
    {
        bool HasUserWithName(string username);
    }
}