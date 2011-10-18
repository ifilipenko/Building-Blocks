using BuildingBlocks.Persistence.Conventions;
using BuildingBlocks.Persistence.Mapping.EnumMap;
using BuildingBlocks.Persistence.Scope;
using NHibernate;
using StructureMap;

namespace BuildingBlocks.Persistence
{
    public static class PersistenceEnvironment
    {
        public const string SessionKey = "nhibernate.current_session";
        private static ConventionsLocator _conventionsLocator;

        public static void InitializeFactory(ISessionFactory sessionFactory)
        {
            ObjectFactory.Configure(conf => 
                conf.For<ISessionFactory>().Singleton().Use(sessionFactory));
        }

        public static ISessionFactory SessionFactory 
        {
            get { return ObjectFactory.TryGetInstance<ISessionFactory>(); }
        }

        public static bool HasSessionFactory
        {
            get { return SessionFactory != null; }
        }

        public static ISessionLocatorContext CurrentSessionContext
        {
            get 
            {
                return HttpContextSessionLocatorContext.HasContext
                           ? (ISessionLocatorContext) new HttpContextSessionLocatorContext(SessionKey)
                           : new ThreadLocatorContext(SessionKey);
            }
        }

        internal static EnumMapContainer GetEnumMapContainer()
        {
            return ObjectFactory.TryGetInstance<EnumMapContainer>();
        }

        public static ConventionsLocator ConventionsLocator
        {
            get { return _conventionsLocator ?? (_conventionsLocator = new ConventionsLocator()); }
        }
    }
}