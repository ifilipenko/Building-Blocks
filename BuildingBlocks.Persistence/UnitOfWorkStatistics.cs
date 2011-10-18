using BuildingBlocks.Persistence.Scope;
using NHibernate.Stat;

namespace BuildingBlocks.Persistence
{
    public class UnitOfWorkStatistics
    {
        public long GetOpenedSessions()
        {
            var statistics = GetStatistics();
            return statistics.SessionOpenCount - statistics.SessionCloseCount;
        }

        public long GetSessionOpenCount()
        {
            var statistics = GetStatistics();
            return statistics.SessionOpenCount;
        }

        public long GetTransactionsCount()
        {
            IStatistics statistics = GetStatistics();
            return statistics.TransactionCount;
        }

        public long GetCollectionLoadCount()
        {
            IStatistics statistics = GetStatistics();
            return statistics.CollectionLoadCount;
        }

        private IStatistics GetStatistics()
        {
            var sessionLocator = SessionLocator.Get();
            return sessionLocator.SessionFactory.Statistics;
        }
    }
}