using BuildingBlocks.Persistence.Exceptions;
using BuildingBlocks.Persistence.Scope;
using NHibernate;

namespace BuildingBlocks.Persistence
{
    public abstract class RepositoryBase
    {
        private readonly SessionLocator _sessionLocator;
        private SecondLevelCacheSetup _cache;

        protected RepositoryBase()
        {
            _sessionLocator = SessionLocator.Get();
        }

        public SecondLevelCacheSetup Cache
        {
            get { return _cache ?? (_cache = new SecondLevelCacheSetup(CurrentSession)); }
        }

        protected ISession CurrentSession
        {
            get
            {
                var session = _sessionLocator.Session;
                if (session == null)
                {
                    throw new UnitOfWorkScopeNotOpenException();
                }
                return session;
            }
        }
    }
}