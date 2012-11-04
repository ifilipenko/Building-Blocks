using Raven.Client;

namespace BuildingBlocks.Store.RavenDB
{
    public class RavenDbStorage : IStorage
    {
        private readonly IDocumentStore _documentStore;
        private RavenDbSessionSettings _sessionSettings;

        public RavenDbStorage(IDocumentStore documentStore, RavenDbSessionSettings sessionSettings = null)
        {
            _documentStore = documentStore;
            SessionSettings = sessionSettings;
        }

        public IDocumentStore DocumentStore
        {
            get { return _documentStore; }
        }

        public RavenDbSessionSettings SessionSettings
        {
            get { return _sessionSettings; }
            set
            {
                _sessionSettings = value ?? new RavenDbSessionSettings();
            }
        }

        public IStorageSession OpenSesion()
        {
            return new RavenDbSession(_documentStore, _sessionSettings);
        }
    }
}