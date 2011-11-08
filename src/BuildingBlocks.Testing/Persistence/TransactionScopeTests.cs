using System;
using System.Threading;
using BuildingBlocks.Common;
using BuildingBlocks.Persistence;
using BuildingBlocks.Persistence.Exceptions;
using BuildingBlocks.Persistence.Scope;
using BuildingBlocks.Testing.Persistence.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Testing.Persistence
{
#pragma warning disable 168
    [TestClass]
    public class TransactionScopeTests : TestBase
    {
        [TestMethod]
        public void should_create_and_dispose_transaction()
        {
            var beforeTransactionsCount = UnitOfWork.GetStatistics().GetTransactionsCount();

            using (var uow = UnitOfWork.TransactionScope())
            {
                uow.Should().NotBeNull();
                uow.Session.Should().NotBeNull();
                uow.Session.Transaction.Should().NotBeNull();
                SessionLocator.Get().HasAliveTransaction.Should().BeTrue();
            }

            UnitOfWork.GetStatistics().GetTransactionsCount().Should().Be(beforeTransactionsCount + 1);
            SessionLocator.Get().HasAliveTransaction.Should().BeFalse();
        }

        [TestMethod]
        public void should_rollback_included_not_committed_transactionscopes_after_dispose_deepest_included_transaction()
        {
            using (UnitOfWork.TransactionScope())
            {
                using (UnitOfWork.TransactionScope())
                {
                    using (UnitOfWork.TransactionScope())
                    {
                        SessionLocator.Get().HasAliveTransaction.Should().BeTrue();
                    }

                    SessionLocator.Get().HasAliveTransaction.Should().BeFalse();
                }

                SessionLocator.Get().HasAliveTransaction.Should().BeFalse();
            }

            SessionLocator.Get().HasAliveTransaction.Should().BeFalse();
        }

        [TestMethod]
        public void should_not_close_session_when_dispose_deepest_included_transaction()
        {
            using (UnitOfWork.TransactionScope())
            {
                using (UnitOfWork.TransactionScope())
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
            using (UnitOfWork.TransactionScope())
            {
                var now = MicDateTime.Now;
                SessionLocator.Get().Session.IsOpen
                    .Should().BeTrue();
            }
        }

        [TestMethod]
        public void should_include_many_transactionscopes_with_same_NHibernateTransaction()
        {
            using (var uow1 = UnitOfWork.TransactionScope())
            {
                using (var uow2 = UnitOfWork.TransactionScope())
                {
                    using (var uow3 = UnitOfWork.TransactionScope())
                    {
                        var t1 = uow1.Session.Transaction;
                        var t2 = uow2.Session.Transaction;
                        var t3 = uow3.Session.Transaction;

                        t1.Should().BeSameAs(t2);
                        t1.Should().BeSameAs(t3);
                        t2.Should().BeSameAs(t3);
                    }
                }
            }
        }

        [TestMethod]
        public void should_open_new_transaction_after_rolledback_transaction()
        {
            using (var upperUow = UnitOfWork.Scope())
            {
                var repository = new Repository();

                using (var innerTxUow1 = UnitOfWork.TransactionScope())
                {
                    var tariffPlan = new TariffPlan { Name = "New 1" };
                    repository.Save(tariffPlan);
                }

                using (var innerTxUow2 = UnitOfWork.TransactionScope())
                {
                    var transaction = innerTxUow2.Session.Transaction;
                    transaction.WasCommitted.Should().BeFalse();
                    transaction.WasRolledBack.Should().BeFalse();
                    transaction.IsActive.Should().BeTrue();
                }
            }
        }

        [TestMethod]
        public void should_fail_if_after_rolledback_inner_tansaction_call_submitChanges_in_upper_transactionscope()
        {
            using (var upperTxScope = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();

                using (var innerTxScope = UnitOfWork.TransactionScope())
                {
                    var tariffPlan = new TariffPlan { Name = "New" };
                    repository.Save(tariffPlan);
                }

                Action action = upperTxScope.SubmitChanges;
                action.ShouldThrow<TransactionWasRolledBackException>();
            }
        }

        [TestMethod]
        public void should_not_fail_when_after_rolledback_all_transactions_invoke_SubmitChanges()
        {
            using (var uow = UnitOfWork.Scope())
            {
                using (UnitOfWork.TransactionScope())
                {
                    using (UnitOfWork.TransactionScope())
                    {
                        var repository = new Repository();
                        var tariffPlan = new TariffPlan { Name = "New" };
                        repository.Save(tariffPlan);
                    }
                }
                uow.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_commit_transaction_when_all_included_transactionscopes_committed()
        {
            using (var upperTransactionScope = UnitOfWork.TransactionScope())
            {
                var nhibTransaction = upperTransactionScope.Session.Transaction;

                using (var innerTransactionScope = UnitOfWork.TransactionScope())
                {
                    var tariffPlan = new TariffPlan { Name = "New" };
                    new Repository().Save(tariffPlan);

                    innerTransactionScope.SubmitChanges();

                    nhibTransaction.WasCommitted.Should().BeFalse();
                    nhibTransaction.WasRolledBack.Should().BeFalse();
                    nhibTransaction.IsActive.Should().BeTrue();
                }

                upperTransactionScope.SubmitChanges();

                nhibTransaction.WasCommitted.Should().BeTrue();
                nhibTransaction.WasRolledBack.Should().BeFalse();
                nhibTransaction.IsActive.Should().BeFalse();
            }
        }

        [TestMethod]
        public void should_rollback_changes_when_commited_only_InnerTransaction()
        {
            using (var upperTransactionScope = UnitOfWork.TransactionScope())
            {
                var nhibTransaction = upperTransactionScope.Session.Transaction;

                using (var innerTransactionScope = UnitOfWork.TransactionScope())
                {
                    var tariffPlan = new TariffPlan { Name = "New" };
                    new Repository().Save(tariffPlan);

                    innerTransactionScope.SubmitChanges();

                    nhibTransaction.WasCommitted.Should().BeFalse();
                    nhibTransaction.WasRolledBack.Should().BeFalse();
                    nhibTransaction.IsActive.Should().BeTrue();
                }

                nhibTransaction.WasCommitted.Should().BeFalse();
                nhibTransaction.WasRolledBack.Should().BeFalse();
                nhibTransaction.IsActive.Should().BeTrue();
            }
        }

        [TestMethod]
        public void should_not_submit_changes_if_upper_transaction_not_comitted()
        {
            long countBeforeTransaction;
            using (var upperTransactionScope = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();
                countBeforeTransaction = repository.GetCount<TariffPlan>();

                using (var innerScope = UnitOfWork.Scope())
                {
                    var tariffPlan = new TariffPlan { Name = "New" };
                    repository.Save(tariffPlan);

                    innerScope.SubmitChanges();
                }
            }

            long countAfterTransaction;
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                countAfterTransaction = repository.GetCount<TariffPlan>();
            }

            countAfterTransaction.Should().Be(countBeforeTransaction);
        }

        [TestMethod]
        public void should_submit_changes_nottransactional_scope_if_upper_transactionscope_was_comitted()
        {
            long countBeforeTransaction;
            using (var upperTransactionScope = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();
                countBeforeTransaction = repository.GetCount<TariffPlan>();

                using (var innerScope = UnitOfWork.Scope())
                {
                    var tariffPlan = new TariffPlan { Name = "New" };
                    repository.Save(tariffPlan);

                    innerScope.SubmitChanges();
                }

                upperTransactionScope.SubmitChanges();
            }

            long countAfterTransaction;
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                countAfterTransaction = repository.GetCount<TariffPlan>();
            }

            countAfterTransaction.Should().Be(countBeforeTransaction + 1);
        }

        [TestMethod]
        public void should_can_save_in_each_included_transaction()
        {
            long countBeforeTransaction;
            using (var txScope1 = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();
                countBeforeTransaction = repository.GetCount<TariffPlan>();

                var tariffPlan1 = new TariffPlan { Name = "New1" };
                repository.Save(tariffPlan1);

                using (var txScope2 = UnitOfWork.TransactionScope())
                {
                    var tariffPlan2 = new TariffPlan { Name = "New2" };
                    repository.Save(tariffPlan2);

                    using (var txScope3 = UnitOfWork.TransactionScope())
                    {
                        var tariffPlan3 = new TariffPlan { Name = "New3" };
                        repository.Save(tariffPlan3);

                        txScope3.SubmitChanges();
                    }

                    txScope2.SubmitChanges();
                }

                txScope1.SubmitChanges();
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
            var beforeTestTransCount = UnitOfWork.GetStatistics().GetTransactionsCount();
            int trans1Hash;
            int trans2Hash = 0;
            int trans3Hash = 0;

            using (var txScope1 = UnitOfWork.TransactionScope())
            {
                var t1 = txScope1.Session.Transaction;
                trans1Hash = t1.GetHashCode();
            }

            ThreadStart doThread2 = () =>
            {
                using (var txScope2 = UnitOfWork.TransactionScope())
                {
                    var t2 = txScope2.Session.Transaction;
                    trans2Hash = t2.GetHashCode();
                }

                ThreadStart doThread3 = () =>
                {
                    using (var txScope3 = UnitOfWork.TransactionScope())
                    {
                        var t3 = txScope3.Session.Transaction;
                        trans3Hash = t3.GetHashCode();
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

            var afterTestTransCount = UnitOfWork.GetStatistics().GetTransactionsCount();
            afterTestTransCount.Should().Be(beforeTestTransCount + 3);
            trans1Hash.Should().NotBe(trans2Hash);
            trans1Hash.Should().NotBe(trans3Hash);
            trans2Hash.Should().NotBe(trans3Hash);
        }
    }
    #pragma warning restore 168
}
