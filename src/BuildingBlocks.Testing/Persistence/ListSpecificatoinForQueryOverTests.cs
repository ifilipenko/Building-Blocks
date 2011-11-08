using System.Linq;
using BuildingBlocks.Common.ListSpecification;
using BuildingBlocks.Persistence;
using BuildingBlocks.Persistence.Helpers;
using BuildingBlocks.Testing.Persistence.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Testing.Persistence
{
    [TestClass]
    public class ListSpecificatoinForQueryOverTests : TestBase
    {
        [TestInitialize]
        public void setup()
        {
            using (var uow = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();
                var tariffPlans = Enumerable.Range(1, 10)
                    .Select(i => new TariffPlan
                                     {
                                         Name = "Tariff plan " + i
                                     }).ToList();

                tariffPlans.Last().Name = "Last item";
                foreach (var tariffPlan in tariffPlans)
                {
                    repository.Save(tariffPlan);
                }
                uow.SubmitChanges();
            }
        }

        [TestCleanup]
        public void teardown()
        {
            using (var uow = UnitOfWork.TransactionScope())
            {
                uow.Session.Delete("from " + typeof(object));
                uow.SubmitChanges();
            }
        }

        [TestMethod]
        public void should_filter_without_paging_by_specification()
        {
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                var listSpec = new ListSpecification<TariffPlan>();
                listSpec.AddContains(e => e.Name, "Tariff");

                var plans = repository.QueryOver<TariffPlan>().WithListSpecification(listSpec).ToList();

                plans.Should().HaveCount(9);
            }
        }
    }
}