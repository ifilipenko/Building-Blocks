using BuildingBlocks.Store;

namespace BuildingBlocks.Membership.RavenDB
{
    public abstract class RepositoryBase
    {
        private readonly IStorageSession _outsideSession;
        private readonly IStorage _storage;

        protected RepositoryBase(IStorage storage)
        {
            _storage = storage;
        }

        protected RepositoryBase(IStorageSession outsideSession)
        {
            _outsideSession = outsideSession;
        }

        protected IStorageSession OpenSesion()
        {
            return _storage == null 
                       ? new OutsideSessionDecorator(_outsideSession) 
                       : _storage.OpenSesion();
        }
    }
}