using System.Linq;
using Raven.Client;
using Raven.Client.Embedded;
using TechTalk.SpecFlow;

namespace BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow
{
    [Binding]
    public class RavenDbManagement
    {
        private static IDocumentStore _store;

        [BeforeScenario()]
        public void CreateRavenDb()
        {
            RavenDb.Store = CreateStorage();
        }

        [AfterScenario]
        public void DisposeRavenDb()
        {
            RavenDb.DisposeSessions();
            RavenDb.Store.Dispose();
        }

        [BeforeStep]
        public void OpenSessions()
        {
            if (!IsRavenDbStepOrSession)
                return;

            RavenDb.OpenSession();
            RavenDb.OpenStorageSession();
        }

        [AfterStep]
        public void CloseSessions()
        {
            if (ScenarioContext.Current.TestError == null)
            {
                if (RavenDb.HasCurrentSession)
                {
                    RavenDb.CurrentSession.SaveChanges();
                }
                if (RavenDb.HasCurrentStorageSession)
                {
                    RavenDb.CurrentStorageSession.SumbitChanges();
                }
            }
            RavenDb.DisposeSessions();
        }

        private bool IsRavenDbStepOrSession
        {
            get
            {
                var ravenDbTags = new[] { "RavenDb", "RavenDB", "Ravendb", "ravendb", "ravenDb", "ravenDB" };
                var featureInfo = FeatureContext.Current.FeatureInfo;
                var scenarioInfo = ScenarioContext.Current.ScenarioInfo;
                return
                    featureInfo.Tags.Any(t => ravenDbTags.Contains(t.Trim())) ||
                    scenarioInfo.Tags.Any(t => ravenDbTags.Contains(t.Trim()));

            }
        }

        private static IDocumentStore CreateStorage()
        {
            var store = new EmbeddableDocumentStore
                {
                    RunInMemory = true
                };
            store.Initialize();
            return store;
        }
    }
}