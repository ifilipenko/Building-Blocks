using NHibernate;

namespace BuildingBlocks.Persistence.Scope
{
    public class SessionLocatorItem
    {
        public static SessionLocatorItem New()
        {
            return new SessionLocatorItem();
        }

        private int _openedSessions;
        private ISession _session;
        private IStatelessSession _statelessSession;
        private ITransaction _transaction;
        private int _openedTransactions;

        private SessionLocatorItem()
        {
        }

        public int OpenedSessions
        {
            get { return _openedSessions; }
        }

        public int OpenedTransactions
        {
            get { return _openedTransactions; }
        }

        public ISession Session
        {
            get { return _session; }
        }

        public IStatelessSession StatelessSession
        {
            get { return _statelessSession; }
        }

        public ITransaction Transaction
        {
            get { return _transaction; }
        }

        public bool HasAliveTransaction
        {
            get { return _transaction != null && _transaction.IsActive; }
        }

        public SessionLocatorItem OpenInnerSession()
        {
            var clone = Clone();
            clone._openedSessions++;
            return clone;
        }

        public SessionLocatorItem OpenInnerTransaction()
        {
            var clone = Clone();
            clone._openedTransactions++;
            return clone;
        }
        
        public SessionLocatorItem SetNewTransaction(ITransaction transaction)
        {
            var clone = Clone();
            clone._transaction = transaction;
            clone._openedTransactions = 1;
            return clone;
        }

        public SessionLocatorItem OpenNewSession(ISessionFactory sessionFactory)
        {
            var clone = Clone();
            clone._session = sessionFactory.OpenSession();
            clone._openedSessions = 1;
            return clone;
        }

        public SessionLocatorItem CommitTransaction()
        {
            var clone = Clone();
            clone._openedTransactions--;
            if (clone.HasAliveTransaction && clone._openedTransactions < 1)
            {
                clone._transaction.Commit();
            }
            return clone;
        }

        public SessionLocatorItem RollbackTransaction()
        {
            var clone = Clone();
            clone._openedTransactions--;
            if (clone._openedTransactions < 0)
            {
                clone._openedTransactions = 0;
            }
            clone.RollbackTransactionCore();
            return clone;
        }

        public SessionLocatorItem CloseInnerSession()
        {
            var clone = Clone();
            clone._openedSessions--;
            if (clone._openedSessions < 1)
            {
                clone.CloseSessionCore();
                clone._openedSessions = 0;
            }
            return clone;
        }

        public SessionLocatorItem CloseUpperSession()
        {
            var clone = Clone();
            clone.CloseSessionCore();
            return clone;
        }

        private void CloseSessionCore()
        {
            if (_session != null)
            {
                if (_openedTransactions > 0)
                {
                    RollbackTransactionCore();
                    _openedTransactions = 0;
                }

                _session.Dispose();
                _session = null;
            }
            _openedSessions = 0;
        }

        private void RollbackTransactionCore()
        {
            if (HasAliveTransaction)
            {
                _transaction.Rollback();
            }
        }

        private SessionLocatorItem Clone()
        {
            var clone = new SessionLocatorItem
            {
                _openedSessions = _openedSessions,
                _session = _session,
                _statelessSession = _statelessSession,
                _transaction = _transaction,
                _openedTransactions = _openedTransactions
            };
            return clone;
        }
    }
}