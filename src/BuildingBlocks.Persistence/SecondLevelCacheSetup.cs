using NHibernate;

namespace BuildingBlocks.Persistence
{
    public class SecondLevelCacheSetup
    {
        private readonly ISession _session;

        public SecondLevelCacheSetup(ISession session)
        {
            _session = session;
        }

        public void Clear()
        {
            var sessionFactory = _session.SessionFactory;
            sessionFactory.EvictQueries();

            foreach (var collectionMetadata in sessionFactory.GetAllCollectionMetadata())
            {
                sessionFactory.EvictCollection(collectionMetadata.Key);
            }
            foreach (var classMetadata in sessionFactory.GetAllClassMetadata())
            {
                sessionFactory.EvictEntity(classMetadata.Key);
            }
        }

        public void ClearWithRegion(string region)
        {
            var sessionFactory = _session.SessionFactory;
            sessionFactory.EvictQueries(region);
        }
    }
}