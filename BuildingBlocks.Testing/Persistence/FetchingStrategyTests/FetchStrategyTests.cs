using System.Linq;
using BuildingBlocks.Persistence.Fetching;
using BuildingBlocks.Persistence.Scope;
using BuildingBlocks.Testing.Persistence.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Linq;

namespace BuildingBlocks.Testing.Persistence.FetchingStrategyTests
{
    [TestClass]
    public class FetchStrategyTests : FetchTestBase
    {
        [TestMethod]
        public void should_concat_strategy_with_other_strategy()
        {
            var tariffPlanFetchStrategy = new FetchStrategy<TariffPlan>();
            tariffPlanFetchStrategy.FetchMany(tp => tp.Versions);

            var fetchStrategy = new FetchStrategy<Contract>();
            fetchStrategy.FetchMany(c => c.TariffPlans).ConcatWith(tariffPlanFetchStrategy);

            var firstLevel = fetchStrategy.Fetches;
            firstLevel.Should().HaveCount(1);

            var secondLevel = firstLevel.First().RelatedFetches;
            secondLevel.Should().HaveCount(1);
        }

        [TestMethod]
        public void should_correctly_build_fetching_tree()
        {
            var fetchStrategy = new FetchStrategy<Contract>();
            fetchStrategy.FetchMany(c => c.TariffPlans).ThenFetchMany(tp => tp.Versions);

            var firstLevel = fetchStrategy.Fetches;
            firstLevel.Should().HaveCount(1);

            var secondLevel = firstLevel.First().RelatedFetches;
            secondLevel.Should().HaveCount(1);
        }

        [TestMethod]
        public void should_ignore_add_same_fetching_twice()
        {
            var fetchStrategy = new FetchStrategy<Contract>();
            fetchStrategy.FetchMany(c => c.TariffPlans).ThenFetchMany(tp => tp.Versions);
            fetchStrategy.FetchMany(c => c.TariffPlans).ThenFetchMany(tp => tp.Versions);

            var firstLevel = fetchStrategy.Fetches;
            firstLevel.Should().HaveCount(1);

            var secondLevel = firstLevel.First().RelatedFetches;
            secondLevel.Should().HaveCount(1);
        }

        [TestMethod]
        public void should_correctly_concat_fetch_tree_with_other_fetch_strategy()
        {
            var tariffPlanFetchStrategy = new FetchStrategy<TariffPlan>();
            tariffPlanFetchStrategy.FetchMany(tp => tp.Versions);

            var fetchStrategy = new FetchStrategy<Contract>();
            fetchStrategy.FetchMany(c => c.TariffPlans).ThenFetchMany(tp => tp.Versions);
            fetchStrategy.FetchMany(c => c.TariffPlans).ConcatWith(tariffPlanFetchStrategy);

            var firstLevel = fetchStrategy.Fetches;
            firstLevel.Should().HaveCount(1);

            var secondLevel = firstLevel.First().RelatedFetches;
            secondLevel.Should().HaveCount(1);
        }

        [TestMethod]
        public void apply_to_should_apply_fetches_tree()
        {
            var tariffPlanFetchStrategy = new FetchStrategy<TariffPlan>();
            tariffPlanFetchStrategy.FetchMany(tp => tp.Versions);

            var fetchStrategy = new FetchStrategy<Contract>();
            fetchStrategy.FetchMany(c => c.TariffPlans).ConcatWith(tariffPlanFetchStrategy);

            using (var session = SessionLocator.Get().SessionFactory.OpenSession())
            {
                var queryable = session.Query<Contract>();
                queryable = fetchStrategy.ApplyTo(queryable);
                var result = queryable.ToList();

                NHibernateUtil.IsPropertyInitialized(result.First(), "TariffPlans").Should().BeTrue();
                NHibernateUtil.IsInitialized(result.First().TariffPlans).Should().BeTrue();
                foreach (var tariffPlan in result.First().TariffPlans)
                {
                    NHibernateUtil.IsPropertyInitialized(result.First(), "Versions").Should().BeTrue();
                    NHibernateUtil.IsInitialized(tariffPlan.Versions).Should().BeTrue();

                    foreach (var version in tariffPlan.Versions)
                    {
                        NHibernateUtil.IsInitialized(version).Should().BeTrue();
                    }
                }
            }
        }
    }
}