using Raven.Client;

namespace BuildingBlocks.Store.RavenDB
{
    public class RavenDbStorage : IStorage
    {
        private readonly IDocumentStore _documentStore;

        public RavenDbStorage(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public IStorageSession OpenSesion()
        {
            return new RavenDbSession(_documentStore);
        }
    }
}