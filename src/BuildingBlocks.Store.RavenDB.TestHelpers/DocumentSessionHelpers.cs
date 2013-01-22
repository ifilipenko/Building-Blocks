using System.Threading;
using Raven.Client;

namespace BuildingBlocks.Store.RavenDB.TestHelpers
{
    public static class DocumentSessionHelpers
    {
        public static void ClearStaleIndexes(this IDocumentSession documentSession)
        {
            while (documentSession.Advanced.DatabaseCommands.GetStatistics().StaleIndexes.Length != 0)
            {
                Thread.Sleep(10);
            }
        }
    }
}
