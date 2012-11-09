using Raven.Client;
using Raven.Client.Indexes;

namespace BuildingBlocks.Membership.RavenDB
{
    public class ProvidersIndexes
    {
        private static bool _indexesCreated;
        private static readonly object _lockObject = new object();

        public static void Ensure(IDocumentStore documentStore)
        {
            if (!_indexesCreated)
            {
                lock (_lockObject)
                {
                    if (!_indexesCreated)
                    {
                        EnsureCore(documentStore);
                    }
                }
                _indexesCreated = true;
            }
        }

        private static void EnsureCore(IDocumentStore documentStore)
        {
            IndexCreation.CreateIndexes(typeof (ProvidersIndexes).Assembly, documentStore);
        }
    }
}