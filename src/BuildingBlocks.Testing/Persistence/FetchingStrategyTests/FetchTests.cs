using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class FetchTests : FetchTestBase
    {
        [TestMethod]
        public void fetch_from_root_should_correct_apply_to_query()
        {
            Expression<Func<Service, ServiceType>> expression = s => s.ServiceType;
            var fetch = new Fetch(expression);

            using (var session = SessionLocator.Get().SessionFactory.OpenSession())
            {
                var queryable = session.Query<Service>();
                var queryWithFetch = fetch.ApplyFetch(queryable);
                var result = queryWithFetch.ToList();

                NHibernateUtil.IsInitialized(result.First().ServiceType)
                    .Should().BeTrue();
            }
        }

        [TestMethod]
        public void fetch_many_from_child_should_correct_apply_to_query()
        {
            Expression<Func<Contract, IEnumerable<TariffPlan>>> tariffPlansSelector = s => s.TariffPlans;
            Expression<Func<TariffPlan, IEnumerable<TariffPlanVersion>>> versionsSelector = st => st.Versions;
            var fetch = new Fetch(tariffPlansSelector);
            fetch.AddFetch(new Fetch(versionsSelector));

            using (var session = SessionLocator.Get().SessionFactory.OpenSession())
            {
                var queryable = session.Query<Contract>();
                var queryWithFetch = fetch.ApplyFetch(queryable);
                var result = queryWithFetch.ToList();

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

        [TestMethod]
        public void fetch_many_from_root_should_correct_apply_to_query()
        {
            Expression<Func<TariffPlan, IEnumerable<TariffPlanVersion>>> expression = tp => tp.Versions;
            var fetch = new Fetch(expression);

            using (var session = SessionLocator.Get().SessionFactory.OpenSession())
            {
                var queryable = session.Query<TariffPlan>();
                var queryWithFetch = fetch.ApplyFetch(queryable);
                var result = queryWithFetch.ToList();

                NHibernateUtil.IsInitialized(result.First().Versions).Should().BeTrue();
                foreach (var version in result.First().Versions)
                {
                    NHibernateUtil.IsInitialized(version).Should().BeTrue();
                }
            }
        }
    }
}