using System;
using System.Collections.Generic;
using BuildingBlocks.Persistence.Scope;
using BuildingBlocks.Testing.Persistence.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Testing.Persistence.FetchingStrategyTests
{
    [TestClass]
    public class FetchTestBase : TestBase
    {
        [TestInitialize]
        public void setup()
        {
            using (var session = SessionLocator.Get().SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var tariffPlan = new TariffPlan { Name = "New 1" };
                tariffPlan.AddVersion(1, DateTime.Now, DateTime.Now.AddDays(1));
                tariffPlan.AddVersion(2, DateTime.Now, DateTime.Now.AddDays(1));
                session.Save(tariffPlan);

                var country = new Country { Name = "Russian" };
                session.Save(country);

                var serviceType = new ServiceType
                                      {
                                          FederalCode = "22",
                                          LocalCode = "22",
                                          Name = "service",
                                          Number = 11,
                                          Currency = new Currency
                                                         {
                                                             Sign = "p",
                                                             ShortName = "Rub",
                                                             FullName = "Ruble",
                                                             Country = country
                                                         }
                                      };
                session.Save(serviceType);

                var service = new Service();
                session.Save(service);

                var contract = new Contract { TariffPlans = new List<TariffPlan>() };
                session.Save(contract);

                transaction.Commit();
            }

            Console.WriteLine("************************* setup end ************************");
        }

        [TestCleanup]
        public void teardown()
        {
            Console.WriteLine("************************* teardown start ************************");

            using (var session = SessionLocator.Get().SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Delete("from " + typeof(TariffPlan));
                session.Delete("from " + typeof(Contract));
                session.Delete("from " + typeof(Service));
                session.Delete("from " + typeof(ServiceType));
                session.Delete("from " + typeof(Country));

                transaction.Commit();
            }
        }
    }
}