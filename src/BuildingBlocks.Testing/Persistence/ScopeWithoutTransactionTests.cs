using System.Linq;
using System.Threading;
using BuildingBlocks.Common;
using BuildingBlocks.Persistence;
using BuildingBlocks.Persistence.Scope;
using BuildingBlocks.Testing.Persistence.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Testing.Persistence
{
    [TestClass]
    public class ScopeWithoutTransactionTests : TestBase 
    {
        [TestCleanup]
        public void teardown()
        {
            using (var session = SessionLocator.Get().SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Delete("from " + typeof(TariffPlan));

                transaction.Commit();
            }
        }

        [TestMethod]
        public void should_create_and_dispose_scope_without_transaction()
        {
            var beforeTransactions = UnitOfWork.GetStatistics().GetTransactionsCount();

            using (var uow = UnitOfWork.Scope())
            {
                uow.Should().NotBeNull();
                uow.Session.Should().NotBeNull();
                uow.Session.Transaction.Should().NotBeNull();
                SessionLocator.Get().HasAliveTransaction.Should().BeFalse();
            }

            UnitOfWork.GetStatistics().GetTransactionsCount().Should().Be(beforeTransactions);
            SessionLocator.Get().HasAliveTransaction.Should().BeFalse();
        }

        [TestMethod]
        public void should_open_and_dispose_one_session()
        {
            var beforeSessions = UnitOfWork.GetStatistics().GetSessionOpenCount();

            using (var uow = UnitOfWork.Scope())
            {
                uow.Session.Should().NotBeNull();
            }

            UnitOfWork.GetStatistics().GetSessionOpenCount().Should().Be(beforeSessions + 1);
            SessionLocator.Get().Session.Should().BeNull();
        }

        [TestMethod]
        public void should_include_many_copes_with_same_session()
        {
            var beforeSessions = UnitOfWork.GetStatistics().GetSessionOpenCount();

            using (var uow1 = UnitOfWork.Scope())
            {
                using (var uow2 = UnitOfWork.Scope())
                {
                    using (var uow3 = UnitOfWork.Scope())
                    {
                        var s1 = uow1.Session;
                        var s2 = uow2.Session;
                        var s3 = uow3.Session;

                        s1.Should().BeSameAs(s2);
                        s1.Should().BeSameAs(s3);
                        s2.Should().BeSameAs(s3);
                    }
                }
            }

            UnitOfWork.GetStatistics().GetSessionOpenCount().Should().Be(beforeSessions + 1);
        }

        [TestMethod]
        public void should_not_close_session_after_dispose_deepest_included_transaction()
        {
            using (UnitOfWork.Scope())
            {
                using (UnitOfWork.Scope())
                {
                    SessionLocator.Get().Session.IsOpen
                        .Should().BeTrue();
                }
                SessionLocator.Get().Session.IsOpen
                    .Should().BeTrue();
            }
        }

        [TestMethod]
        public void should_not_close_session_when_MicDateTimeNow_executed()
        {
            using (UnitOfWork.Scope())
            {
                var now = MicDateTime.Now;
                SessionLocator.Get().Session.IsOpen
                    .Should().BeTrue();
            }
        }

        [TestMethod]
        public void should_submit_changes_in_upper_scope_when_inner_scope_has_not_submited_changes()
        {
            using (var upperUow = UnitOfWork.Scope())
            {
                var repository = new Repository();

                using (UnitOfWork.Scope())
                {
                    var tariffPlan = new TariffPlan { Name = "New 1" };
                    repository.Save(tariffPlan);
                }

                upperUow.SubmitChanges();
            }

            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                repository.GetCount<TariffPlan>().Should().Be(1L);
            }
        }

        [TestMethod]
        public void should_submit_changes_in_inner_scope_and_ignore_all_unsubmitted_changes_after_it_scope()
        {
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();

                var tariffPlan1 = new TariffPlan { Name = "New1" };
                repository.Save(tariffPlan1);

                using (var innerScope = UnitOfWork.Scope())
                {
                    var tariffPlan2 = new TariffPlan { Name = "New2" };
                    repository.Save(tariffPlan2);

                    innerScope.SubmitChanges();
                }

                tariffPlan1 = repository.GetByID<TariffPlan>(tariffPlan1.TariffPlanID);
                tariffPlan1.Name = "New 3";
                repository.Save(tariffPlan1);
            }

            using (UnitOfWork.Scope())
            {
                var repository = new Repository();

                repository.GetCount<TariffPlan>().Should().Be(2L);
                repository.Query<TariffPlan>()
                    .Where(t => t.Name.EndsWith("3"))
                    .LongCount()
                    .Should().Be(0L);
            }
        }

        [TestMethod]
        public void should_can_save_in_each_included_scopes()
        {
            long countBeforeTransaction;
            using (var scope1 = UnitOfWork.Scope())
            {
                var repository = new Repository();
                countBeforeTransaction = repository.GetCount<TariffPlan>();

                var tariffPlan1 = new TariffPlan { Name = "New1" };
                repository.Save(tariffPlan1);

                scope1.SubmitChanges();

                using (var scope2 = UnitOfWork.Scope())
                {
                    var tariffPlan2 = new TariffPlan { Name = "New2" };
                    repository.Save(tariffPlan2);

                    scope2.SubmitChanges();

                    using (var scope3 = UnitOfWork.Scope())
                    {
                        var tariffPlan3 = new TariffPlan { Name = "New3" };
                        repository.Save(tariffPlan3);

                        scope3.SubmitChanges();
                    }
                }
            }

            long countAfterTransaction;
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                countAfterTransaction = repository.GetCount<TariffPlan>();
            }

            countAfterTransaction.Should().Be(countBeforeTransaction + 3);
        }

        [TestMethod]
        public void should_open_in_different_scope_for_each_thread()
        {
            var beforeTestSessionsCount = UnitOfWork.GetStatistics().GetSessionOpenCount();
            int session1Hash;
            int session2Hash = 0;
            int session3Hash = 0;

            using (var scope1 = UnitOfWork.Scope())
            {
                session1Hash = scope1.Session.GetHashCode();
            }

            ThreadStart doThread2 = () =>
            {
                using (var scope2 = UnitOfWork.Scope())
                {
                    session2Hash = scope2.Session.GetHashCode();
                }

                ThreadStart doThread3 = () =>
                {
                    using (var scope3 = UnitOfWork.Scope())
                    {
                        session3Hash = scope3.Session.GetHashCode();
                    }
                };

                var thread3 = new Thread(doThread3);
                thread3.Start();
                while (thread3.IsAlive)
                {
                    Thread.Sleep(100);
                }
            };

            var thread2 = new Thread(doThread2);
            thread2.Start();
            while (thread2.IsAlive)
            {
                Thread.Sleep(100);
            }

            var afterTestSessionsCount = UnitOfWork.GetStatistics().GetSessionOpenCount();
            afterTestSessionsCount.Should().Be(beforeTestSessionsCount + 3);

            session1Hash.Should().NotBe(session2Hash);
            session1Hash.Should().NotBe(session3Hash);
            session2Hash.Should().NotBe(session3Hash);
        }
    }
}