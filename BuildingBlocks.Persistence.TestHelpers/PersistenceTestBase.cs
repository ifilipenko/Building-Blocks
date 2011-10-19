using System.Collections.Generic;
using BuildingBlocks.Persistence.SQLite;
using BuildingBlocks.Persistence.TestHelpers.TestData;
using BuildingBlocks.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Persistence.TestHelpers
{
    [TestClass]
    public abstract class PersistenceTest : AbstractTest
    {
        private TestDataPersistor _persistor;

        protected virtual void ChangeDbParams(InMemorySqliteConfigurationParameters persistanceParameters)
        {
        }

        sealed protected override IEnumerable<ITestConfiguration> GetConfigurations()
        {
            return new[] { new PersistenceTestConfiguration(this, db => _persistor = db, ChangeDbParams) };
        }

        public TestDataPersistor Db
        {
            get { return _persistor; }
        }
    }
}