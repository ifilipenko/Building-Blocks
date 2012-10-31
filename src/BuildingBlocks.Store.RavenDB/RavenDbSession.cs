using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Linq;

namespace BuildingBlocks.Store.RavenDB
{
    public class RavenDbSession : IStorageSession
    {
        private readonly ILog _log = LogManager.GetLogger<RavenDbSession>();
        private readonly IDocumentStore _documentStore;
        private Lazy<IDocumentSession> _session;
        public readonly Guid Id = Guid.NewGuid();
        private bool _rolledBack;

        public RavenDbSession(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public RavenDbSession(IDocumentSession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            _session = new Lazy<IDocumentSession>(() => session);
        }

        public IDocumentSession Session
        {
            get
            {
                if (_session == null)
                {
                    _session = CreateSessionValue();
                }
                return _session.Value;
            }
        }

        public void Dispose()
        {
            if (IsInitialized)
            {
                _session.Value.Dispose();
                _session = null;
                _log.Debug(m => m("Session with id {0} disposed", Id));
            }
            else
            {
                _log.Debug(m => m("Session is not initialized, disposing ignored"));
            }
        }

        public bool IsInitialized
        {
            get { return _session != null && _session.IsValueCreated && _session.Value != null; }
        }

        public bool IsRolledBack
        {
            get { return _rolledBack; }
        }

        public T GetById<T>(string id, ILoadingStrategy<T> loadingStrategy = null)
        {
            var loader = ApplyLoadingStrategyToSession(Session, loadingStrategy);
            return loader == null ? Session.Load<T>(id) : loader.Load(id);
        }

        public T[] GetByIds<T>(params string[] ids)
        {
            return Session.Load<T>(ids);
        }

        public T[] GetByIds<T>(IEnumerable<string> ids, ILoadingStrategy<T> loadingStrategy = null)
        {
            var loader = ApplyLoadingStrategyToSession(Session, loadingStrategy);
            return loader == null ? Session.Load<T>(ids) : loader.Load(ids.ToArray());
        }

        public T GetById<T>(ValueType id, ILoadingStrategy<T> loadingStrategy = null)
        {
            var loader = ApplyLoadingStrategyToSession(Session, loadingStrategy);
            return loader == null ? Session.Load<T>(id) : loader.Load(id);
        }

        public IQueryable<T> Query<T>(ILoadingStrategy<T> loadingStrategy = null, StaleResultsMode staleResults = StaleResultsMode.AllowStaleResultsMode)
        {
            var query = Session.Query<T>();
            ApplyLoadingStrategyToQuery(query, loadingStrategy);
            if (staleResults == StaleResultsMode.WaitForNonStaleResults)
            {
                query = query
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite(TimeSpan.FromSeconds(5)));
            }
            return query;
        }

        public void Save(IEntity<string> entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                entity.Id = null;
            }
            Session.Store(entity);
        }

        public void Save<TId>(IEntity<TId> entity)
            where TId : struct
        {
            Session.Store(entity);
        }

        public void Delete<T>(T entity)
        {
            Session.Delete(entity);
        }

        public void UseOptimisticConcurrency()
        {
            Session.Advanced.UseOptimisticConcurrency = true;
        }

        public void SumbitChanges()
        {
            if (!IsInitialized)
            {
                _log.Debug(m => m("Session is not initialized, submit changes ignored"));
                return;
            }


            if (_rolledBack)
            {
                throw new InvalidOperationException("Session was rolled back");
            }

            Session.SaveChanges();
            _log.Debug(m => m("All changes saved to RavenDB"));
        }

        public void Rollback()
        {
            _rolledBack = true;
        }

        private Lazy<IDocumentSession> CreateSessionValue()
        {
            return new Lazy<IDocumentSession>(OpenSession, true);
        }

        private IDocumentSession OpenSession()
        {
            var documentSession = _documentStore.OpenSession();
            _log.Debug(m => m("Session with id {0} opened", Id));
            return documentSession;
        }

        private static ILoaderWithInclude<T> ApplyLoadingStrategyToSession<T>(IDocumentSession session, ILoadingStrategy<T> strategy)
        {
            var loadingStrategy = (LoadingStrategy<T>) strategy;
            if (loadingStrategy == null || loadingStrategy.IsEmpty)
                return null;

            var property = loadingStrategy.Dequeue();
            var loaderWithInclude = session.Include(property);
            while (!loadingStrategy.IsEmpty)
            {
                property = loadingStrategy.Dequeue();
                loaderWithInclude = session.Include(property);
            }

            return loaderWithInclude;
        }

        private static void ApplyLoadingStrategyToQuery<T>(IRavenQueryable<T> query, ILoadingStrategy<T> strategy)
        {
            var loadingStrategy = (LoadingStrategy<T>) strategy;
            if (loadingStrategy == null || loadingStrategy.IsEmpty)
                return;

            query.Customize(x =>
                {
                    var property = loadingStrategy.Dequeue();
                    x.Include(property);
                    while (!loadingStrategy.IsEmpty)
                    {
                        property = loadingStrategy.Dequeue();
                        x.Include(property);
                    }
                });
        }

        public void ForcedInitialize()
        {
            Query<object>();
        }
    }
}