using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Store;
using Common.Logging;

namespace BuildingBlocks.Membership.RavenDB
{
    class OutsideSessionDecorator : IStorageSession
    {
        private ILog _log = LogManager.GetLogger<OutsideSessionDecorator>();
        private readonly IStorageSession _outsideSession;
        private Guid _id;

        public OutsideSessionDecorator(IStorageSession outsideSession)
        {
            if (_log.IsDebugEnabled)
            {
                _id = Guid.NewGuid();
            }
            _log.Debug(m => m("Warpper session is opened with an id {0}", _id));
            _outsideSession = outsideSession;
        }

        public IStorageSession OutsideSession
        {
            get { return _outsideSession; }
        }

        public bool IsRolledBack
        {
            get { return _outsideSession.IsRolledBack; }
        }

        public void SumbitChanges()
        {
            _log.Debug(m => m("Submit changes is session {0} ignored and need be submitted by outsisde session", _id));
        }

        public void Rollback()
        {
            _outsideSession.Rollback();
            _log.Debug(m => m("Rollback sended to outside session from wrapper {0}", _id));
        }

        public void Dispose()
        {
            _log.Debug(m => m("Wrapper session {0} disposed, but outsisde session disposing was ignored", _id));
        }

        public bool IsInitialized
        {
            get { return _outsideSession.IsInitialized; }
        }

        public T GetById<T>(string id, ILoadingStrategy<T> loadingStrategy = null)
        {
            return _outsideSession.GetById(id, loadingStrategy);
        }

        public T[] GetByIds<T>(params string[] ids)
        {
            return _outsideSession.GetByIds<T>(ids);
        }

        public T[] GetByIds<T>(IEnumerable<string> ids, ILoadingStrategy<T> loadingStrategy = null)
        {
            return _outsideSession.GetByIds<T>(ids, loadingStrategy);
        }

        public T GetById<T>(ValueType id, ILoadingStrategy<T> loadingStrategy = null)
        {
            return _outsideSession.GetById(id, loadingStrategy);
        }

        public IQueryable<T> Query<T>(ILoadingStrategy<T> loadingStrategy = null, StaleResultsMode staleResults = StaleResultsMode.AllowStaleResultsMode)
        {
            return _outsideSession.Query(loadingStrategy, staleResults);
        }

        public void Save(IEntity<string> entity)
        {
            _outsideSession.Save(entity);
        }

        public void Save<TId>(IEntity<TId> entity) where TId : struct
        {
            _outsideSession.Save(entity);
        }

        public void Delete<T>(T entity)
        {
            _outsideSession.Delete(entity);
        }

        public void UseOptimisticConcurrency()
        {
            _outsideSession.UseOptimisticConcurrency();
        }
    }
}