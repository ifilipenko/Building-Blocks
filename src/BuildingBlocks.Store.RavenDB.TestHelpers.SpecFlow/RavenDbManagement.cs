using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Raven.Client;
using Raven.Client.Embedded;
using TechTalk.SpecFlow;

namespace BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow
{
    [Binding]
    public class RavenDbManagement
    {
        private static readonly ILog _log = LogManager.GetLogger<RavenDbManagement>();
        private static IDocumentStore _store;

        [BeforeScenario]
        public void CreateRavenDb()
        {
            RavenDb.InitializeStorage();
        }

        [AfterScenario]
        public void DisposeRavenDb()
        {
            RavenDb.DisposeSessions();
            RavenDb.DisposeStorage();
            _log.Debug(m => m("Document store disposed"));
        }

        [BeforeStep]
        public void OpenSessions()
        {
            if (!IsRavenDbStepOrFeature)
                return;

            RavenDb.OpenSession();
            RavenDb.OpenStorageSession();
            _log.Debug(m => m("Raven db session opened, id={0}", GetCurrentSessionId()));
        }

        [AfterStep]
        public void CloseSessions()
        {
            if (!RavenDb.HasCurrentSession)
                return;

            if (ScenarioContext.Current.TestError == null)
            {
                RavenDb.CurrentStorageSession.SubmitChanges();
                _log.Debug(m => m("All changes saved to raven db, id={0}", GetCurrentSessionId()));
            }

            var id = GetCurrentSessionId();
            RavenDb.DisposeSessions();
            _log.Debug(m => m("Raven Db session closed, id={0}", id));
        }

        private bool IsRavenDbStepOrFeature
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

        private static object GetCurrentSessionId()
        {
            return ((RavenDbSession) RavenDb.CurrentStorageSession).Id;
        }
    }
}