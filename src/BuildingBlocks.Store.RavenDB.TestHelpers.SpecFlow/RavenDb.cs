using Raven.Client;

namespace BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow
{
    public static class RavenDb
    {
        private static IDocumentSession _currentSession;
        private static IStorageSession _currentStorageSession;

        public static IDocumentStore Store { get; set; }

        public static IDocumentSession CurrentSession
        {
            get { return _currentSession ?? (_currentSession = OpenSession()); }
        }

        public static IStorageSession CurrentStorageSession
        {
            get { return _currentStorageSession ?? (_currentStorageSession = OpenStorageSession()); }
        }

        public static bool HasCurrentStorageSession
        {
            get { return _currentStorageSession != null; }
        }

        public static bool HasCurrentSession
        {
            get { return _currentSession != null; }
        }

        public static void DisposeSessions()
        {
            if (_currentSession != null)
            {
                _currentSession.Dispose();
                _currentSession = null;
            }
            if (_currentStorageSession != null)
            {
                _currentStorageSession.Dispose();
                _currentStorageSession = null;
            }
        }

        public static IDocumentSession OpenSession()
        {
            return Store.OpenSession();
        }

        public static IStorageSession OpenStorageSession()
        {
            return new RavenDbSession(Store);
        }
    }
}