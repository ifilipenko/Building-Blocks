using System.Data;
using NHibernate;

namespace BuildingBlocks.Persistence.Scope
{
    public class SessionLocator
    {
        public static SessionLocator Get()
        {
            return new SessionLocator();
        }

        private readonly ISessionLocatorContext _sessionContext;

        private SessionLocator()
        {
            _sessionContext = PersistenceEnvironment.CurrentSessionContext;
        }

        public int OpenedSessionsCount
        {
            get { return CurrentItem.OpenedSessions; }
        }

        public int OpenedTransactionsCount
        {
            get { return CurrentItem.OpenedTransactions; }
        }

        public ISessionFactory SessionFactory 
        {
            get { return PersistenceEnvironment.SessionFactory; }
        }

        public ISession Session
        {
            get { return CurrentItem.Session; }
        }

        public ITransaction Transaction
        {
            get { return CurrentItem.Transaction; }
        }

        public bool ContainLastTransaction
        {
            get { return HasAliveTransaction && CurrentItem.OpenedTransactions == 1; }
        }

        public bool HasAliveTransaction
        {
            get
            {
                return CurrentItem.Transaction != null && CurrentItem.Transaction.IsActive;
            }
        }

        public void OpenSession()
        {
            var currentSession = TryGetCurrentSession();
            if (currentSession == null || CurrentItem.OpenedSessions == 0)
            {
                CurrentItem = CurrentItem.OpenNewSession(PersistenceEnvironment.SessionFactory);
            }
            else
            {
                CurrentItem = CurrentItem.OpenInnerSession();
            }
        }

        public void OpenTransaction(IsolationLevel? isolationLevel)
        {
            if (HasAliveTransaction)
            {
                CurrentItem = CurrentItem.OpenInnerTransaction();
            }
            else
            {
                var transaction = isolationLevel == null
                                      ? Session.BeginTransaction()
                                      : Session.BeginTransaction(isolationLevel.Value);
                CurrentItem = CurrentItem.SetNewTransaction(transaction);
            }
        }

        public void CommitTransaction()
        {
            CurrentItem = CurrentItem.CommitTransaction();
        }

        public void RollbackAliveTransaction()
        {
            CurrentItem = CurrentItem.RollbackTransaction();
        }

        public void SessionDispose()
        {
            CurrentItem = CurrentItem.CloseInnerSession();
        }

        public void ForcedSessionDispose()
        {
            SessionDisposeCore();
        }

        private SessionLocatorItem CurrentItem
        {
            get { return _sessionContext.Item ?? (_sessionContext.Item = SessionLocatorItem.New()); }
            set { _sessionContext.Item = value; }
        }

        private void SessionDisposeCore()
        {
            CurrentItem = CurrentItem.CloseUpperSession();
        }

        private ISession TryGetCurrentSession()
        {
            ISession currentSession;
            try
            {
                currentSession = Session;
            }
            catch (HibernateException)
            {
                currentSession = null;
            }
            return currentSession;
        }
    }
}