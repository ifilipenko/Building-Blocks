using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.TestHelpers
{
    [TestClass]
    public abstract class EntityTestBase
    {
        private DataGenerator.DataGenerator _dataGenerator;

        [TestInitialize]
        public void SetUp()
        {
            _dataGenerator = new DataGenerator.DataGenerator(GenerationRulesAssemblies);
            before_each_tests();
        }

        [TestCleanup]
        public void TearDown()
        {
            after_each_tests();
        }

        protected abstract IEnumerable<Assembly> GenerationRulesAssemblies { get; }

        protected abstract void before_each_tests();
        protected abstract void after_each_tests();

        public DataGenerator.DataGenerator DataGenerator
        {
            get { return _dataGenerator; }
        }
    }
}