using System;
using System.Collections;

namespace BuildingBlocks.Store
{
    public class StorageSessionManager
    {
        private readonly Func<IStorageSession> _sessionFactory;
        private readonly Func<IDictionary> _dataProvider;
        private readonly string _sessionScopeKey;
        private readonly Lazy<ItemStorage<IStorageSession>> _sessionScope;

        public StorageSessionManager(
            Func<IStorageSession> sessionFactory, 
            Func<IDictionary> dataProvider,
            string sessionScopeKey = "storage_session")
        {
            _sessionFactory = sessionFactory;
            _dataProvider = dataProvider;
            _sessionScopeKey = sessionScopeKey;
            _sessionScope = new Lazy<ItemStorage<IStorageSession>>(() => 
                new ItemStorage<IStorageSession>(_dataProvider())
            );
        }

        public ItemStorage<IStorageSession> SessionScope
        {
            get
            {
                return _sessionScope.Value;
            }
        }

        public IStorageSession GetSession()
        {
            return SessionScope.Get(_sessionScopeKey);
        }

        public void OpenSession()
        {
            if (!SessionScope.Contains(_sessionScopeKey))
            {
                SessionScope.Set(_sessionFactory(), _sessionScopeKey);
            }
        }

        public void CloseSession(Exception occuredException)
        {
            if (!SessionScope.Contains(_sessionScopeKey))
                return;

            var session = SessionScope.Get(_sessionScopeKey);
            try
            {
                if (session == null)
                    return;
                using (session)
                {
                    if (occuredException == null && session.IsInitialized && !session.IsCancelled)
                    {
                        session.SumbitChanges();
                    }
                }
            }
            finally
            {
                SessionScope.Remove(_sessionScopeKey);
            }
        }
    }
}