using System;
using FluentAssertions;
using NUnit.Framework;
using Raven.Client.Embedded;

namespace BuildingBlocks.Store.RavenDB.Tests
{
    [TestFixture]
    public class RavenDbSimpleTransactionTests
    {
        private EmbeddableDocumentStore _documentStore;

        [SetUp]
        public void SetupRavenDb()
        {
            _documentStore = new EmbeddableDocumentStore
                                 {
                                     RunInMemory = true
                                 };
            _documentStore.Initialize();
        }

        [Test]
        public void When_Submit_changes_is_not_called_no_changes_is_stored()
        {
            using (var session = new RavenDbSession(_documentStore))
            {
                session.Save(new Employee {Name = "John Smith"});
                session.Save(new Employee {Name = "John Smith Clone1"});
                session.Save(new Employee {Name = "John Smith Clone2"});
            }

            using (var session = new RavenDbSession(_documentStore))
            {
                session.Query<Employee>().Should().HaveCount(0);
            }
        }

        [Test]
        public void When_Submit_changes_is_called_all_changes_is_stored()
        {
            using (var session = new RavenDbSession(_documentStore))
            {
                session.Save(new Employee { Name = "John Smith" });
                session.Save(new Employee { Name = "John Smith Clone1" });
                session.Save(new Employee { Name = "John Smith Clone2" });
                session.SumbitChanges();
            }

            using (var session = new RavenDbSession(_documentStore))
            {
                session.Query<Employee>().Should().HaveCount(3);
            }
        }

        [Test]
        public void When_rollback_called_invocation_of_submit_changes_should_failed()
        {
            using (var session = new RavenDbSession(_documentStore))
            {
                session.Save(new Employee { Name = "John Smith" });
                session.Save(new Employee { Name = "John Smith Clone1" });
                session.Save(new Employee { Name = "John Smith Clone2" });

                session.Rollback();
                Action action = () => session.SumbitChanges();
                action.ShouldThrow<InvalidOperationException>();
            }
        }

        [Test]
        public void When_rollback_called_submit_changes_not_store_changes()
        {
            using (var session = new RavenDbSession(_documentStore))
            {
                session.Save(new Employee {Name = "John Smith"});
                session.Save(new Employee {Name = "John Smith Clone1"});
                session.Save(new Employee {Name = "John Smith Clone2"});

                session.Rollback();
                try
                {
                    session.SumbitChanges();
                }
                catch(InvalidOperationException)
                {
                }
            }

            using (var session = new RavenDbSession(_documentStore))
            {
                session.Query<Employee>().Should().HaveCount(0);
            }
        }

        [Test]
        public void When_rollback_called_and_submit_changes_is_not_called_no_one_exceptions_occured()
        {
            using (var session = new RavenDbSession(_documentStore))
            {
                session.Save(new Employee { Name = "John Smith" });
                session.Save(new Employee { Name = "John Smith Clone1" });
                session.Save(new Employee { Name = "John Smith Clone2" });

                session.Rollback();
            }
        }

        [Test]
        public void When_rollback_called_IsRolledBack_flag_is_true()
        {
            using (var session = new RavenDbSession(_documentStore))
            {
                session.Save(new Employee { Name = "John Smith" });
                session.Save(new Employee { Name = "John Smith Clone1" });
                session.Save(new Employee { Name = "John Smith Clone2" });

                session.Rollback();
                session.IsRolledBack.Should().BeTrue();
            }
        }

        public class Employee : IEntity<int>
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}