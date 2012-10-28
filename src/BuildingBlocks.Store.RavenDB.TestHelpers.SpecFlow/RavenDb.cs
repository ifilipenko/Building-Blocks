using System;
using System.Diagnostics;
using Common.Logging;
using Raven.Client;
using Raven.Client.Embedded;

namespace BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow
{
    public static class RavenDb
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (RavenDb));
        private static IDocumentSession _currentSession;
        private static RavenDbSession _currentStorageSession;
        private static Lazy<IDocumentStore> _documentStore;

        public static IDocumentStore DocumentStore
        {
            get { return _documentStore.Value; }
        }

        public static IStorage Storage
        {
            get { return new RavenDbStorage(DocumentStore); }
        }

        public static IDocumentSession CurrentSession
        {
            get { return _currentSession ?? (_currentSession = OpenSession()); }
        }

        public static IStorageSession CurrentStorageSession
        {
            get { return _currentStorageSession ?? (_currentStorageSession = OpenStorageSession()); }
        }

        public static bool HasCurrentSession
        {
            get { return _currentSession != null; }
        }

        public static void DisposeSessions()
        {
            if (HasCurrentSession)
            {
                CurrentStorageSession.Dispose();
                _currentStorageSession = null;
                _currentSession = null;
            }
        }

        public static IDocumentSession OpenSession()
        {
            return DocumentStore.OpenSession();
        }

        public static RavenDbSession OpenStorageSession()
        {
            var storageSession = new RavenDbSession(CurrentSession);
            if (!storageSession.IsInitialized)
            {
                storageSession.ForcedInitialize();
            }
            return storageSession;
        }

        public static void DisposeStorage()
        {
            if (_documentStore != null && _documentStore.IsValueCreated)
            {
                _documentStore.Value.Dispose();
                LazyInitializeDocumentStore();
            }
        }

        public static void InitializeStorage()
        {
            LazyInitializeDocumentStore();
        }

        private static void LazyInitializeDocumentStore()
        {
            _documentStore = new Lazy<IDocumentStore>(CreateStorage, true);
        }

        private static IDocumentStore CreateStorage()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var store = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };
            store.Initialize();
            stopWatch.Stop();

            _log.Debug(m => m("Document store initilized ({0:F2}s)", stopWatch.Elapsed.TotalSeconds));
            return store;
        }
    }
}