using System;
using System.Data;
using BuildingBlocks.Persistence.Exceptions;
using BuildingBlocks.Persistence.Scope;
using NHibernate;

namespace BuildingBlocks.Persistence
{
    public class UnitOfWork : IDisposable
    {
        internal static UnitOfWork ExistsOrNewScope()
        {
            var sessionLocator = SessionLocator.Get();
            if (sessionLocator.OpenedSessionsCount == 0)
            {
                return new UnitOfWork();
            }

            return new UnitOfWork(openSession: false);
        }

        public static UnitOfWorkStatistics GetStatistics()
        {
            return new UnitOfWorkStatistics();
        }

        public static UnitOfWork Scope()
        {
            var uow = new UnitOfWork {_transactionEnabled = false};
            return uow;
        }

        public static UnitOfWork TransactionScope(IsolationLevel? isolationLevel = null)
        {
            var uow = new UnitOfWork {_transactionEnabled = true};
            uow._sessionLocator.OpenTransaction(isolationLevel);
            return uow;
        }

        private bool _transactionFixed;
        private readonly SessionLocator _sessionLocator;
        private bool _transactionEnabled;

        private UnitOfWork(bool openSession = true)
        {
            if (openSession)
            {
                _sessionLocator = SessionLocator.Get();
                _sessionLocator.OpenSession();
            }
        }

        public void SubmitChanges()
        {
            var session = Session;
            var transaction = _sessionLocator.Transaction;
            if (_transactionEnabled && transaction != null)
            {
                if (transaction.WasRolledBack)
                    throw new TransactionWasRolledBackException();

                _sessionLocator.CommitTransaction();
                _transactionFixed = true;
            }
            else
            {
                session.Flush();
            }
        }

        public void Dispose()
        {
            try
            {
                var session = Session;
                bool hasNotCommitedTransaction =
                    _transactionEnabled &&
                    session.Transaction != null &&
                    session.Transaction.IsActive &&
                    !_transactionFixed;

                if (hasNotCommitedTransaction)
                {
                    _sessionLocator.RollbackAliveTransaction();
                }
            }
            finally
            {
                _sessionLocator.SessionDispose();
            }
        }

        public ISession Session
        {
            get { return _sessionLocator.Session; }
        }
    }
}