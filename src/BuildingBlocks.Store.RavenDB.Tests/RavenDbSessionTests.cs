using FluentAssertions;
using NUnit.Framework;
using Raven.Client.Embedded;

namespace BuildingBlocks.Store.RavenDB.Tests
{
    [TestFixture]
    public class RavenDbSessionTests
    {
        private EmbeddableDocumentStore _documentStore;
        private RavenDbSession _session;

        [TestFixtureSetUp]
        public  void SetupRavenDb()
        {
            _documentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true
                };
            _documentStore.Initialize();
        }

        [SetUp]
        public void SetupSession()
        {
            _session = new RavenDbSession(_documentStore);
        }

        [TearDown]
        public void DisposeSession()
        {
            _session.Dispose();
        }

        [Test]
        public void RavenDbSessionShouldSaveEntityWithStringId()
        {
            var entityWithStringId = new EntityWithStringId();
            _session.Save(entityWithStringId);
            entityWithStringId.Id.Should().Be("EntityWithStringIds/1");
        }

        [Test]
        public void RavenDbSessionShouldUpdateEntityWithStringId()
        {
            var entityWithLongId = new EntityWithLongId();
            _session.Save(entityWithLongId);
            entityWithLongId.Id.Should().Be(1L);
        }

        private class EntityWithLongId : IEntity<long>
        {
            public long Id { get; set; }
        }

        private class EntityWithStringId : IEntity<string>
        {
            public string Id { get; set; }
        }
    }
}