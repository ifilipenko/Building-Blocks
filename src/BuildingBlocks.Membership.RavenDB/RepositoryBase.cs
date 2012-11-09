using BuildingBlocks.Store;
using BuildingBlocks.Store.RavenDB;

namespace BuildingBlocks.Membership.RavenDB
{
    public abstract class RepositoryBase
    {
        private readonly IStorageSession _outsideSession;
        private readonly IStorage _storage;

        protected RepositoryBase(IStorage storage)
        {
            _storage = storage;
            ProvidersIndexes.Ensure(((RavenDbStorage)_storage).DocumentStore);
        }

        protected RepositoryBase(IStorageSession outsideSession)
        {
            _outsideSession = outsideSession;
            ProvidersIndexes.Ensure(((RavenDbSession)_storage).Session.Advanced.DocumentStore);
        }

        protected IStorageSession OpenSesion()
        {
            return _storage == null 
                       ? new OutsideSessionDecorator(_outsideSession) 
                       : _storage.OpenSesion();
        }
    }
}