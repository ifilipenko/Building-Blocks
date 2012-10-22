using BuildingBlocks.Membership.Contract;

namespace BuildingBlocks.Membership
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
}