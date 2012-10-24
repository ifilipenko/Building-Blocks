using Raven.Client;

namespace BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow
{
    public static class RavenDb
    {
        private static IDocumentSession _currentSession;
        private static RavenDbSession _currentStorageSession;

        public static IDocumentStore Store { get; set; }

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
            return Store.OpenSession();
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
    }
}