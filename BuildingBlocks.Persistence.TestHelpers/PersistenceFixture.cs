using System.Collections.Generic;
using BuildingBlocks.Persistence.SQLite;
using BuildingBlocks.Persistence.TestHelpers.TestData;
using BuildingBlocks.TestHelpers;
using BuildingBlocks.TestHelpers.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Persistence.TestHelpers
{
    [TestClass]
    public abstract class PersistenceFixture<T, TWhen, TGiven, TThen> : FixtureBase<T, TWhen, TGiven, TThen> 
        where TWhen : EnvironmentBase, new() 
        where TGiven : ParametersBase, new() 
        where TThen : AssertionsBase, new()
    {
        private TestDataPersistor _persistor;
        
        protected virtual void ChangeDbParams(InMemorySqliteConfigurationParameters persistanceParameters)
        {
        }

        sealed protected override IEnumerable<ITestConfiguration> GetConfigurations()
        {
            return new[] {new PersistenceTestConfiguration(this, db => _persistor = db, ChangeDbParams)};
        }

        public TestDataPersistor Db
        {
            get { return _persistor; }
        }

        protected override TWhen When
        {
            get
            {
                var when = base.When;
                when.Db = _persistor;
                return when;
            }
        }

        protected override TGiven Given
        {
            get
            {
                var given = base.Given;
                given.Db = _persistor;
                return given;
            }
        }

        protected override TThen Then
        {
            get
            {
                var then = base.Then;
                then.Db = _persistor;
                return then;
            }
        }
    }

    public class AssertionsBase : AssertionsFixturePart
    {
        public TestDataPersistor Db { get; internal set; }
    }

    public class ParametersBase : ParametersFixturePart
    {
        public TestDataPersistor Db { get; internal set; }
    }

    public class EnvironmentBase : EnvironmentFixturePart
    {
        public TestDataPersistor Db { get; internal set; }
    }
}