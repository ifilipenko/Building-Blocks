using System;
using System.Collections.Generic;
using BuildingBlocks.Configuration;
using BuildingBlocks.Persistence.SQLite;
using BuildingBlocks.Persistence.TestHelpers.TestData;
using BuildingBlocks.TestHelpers;
using BuildingBlocks.TestHelpers.Dependencies;
using CuttingEdge.Conditions;
using NHibernate.Cache;

namespace BuildingBlocks.Persistence.TestHelpers
{
    public class PersistenceTestConfiguration : ITestConfiguration
    {
        private readonly AbstractTest _test;
        private readonly Action<TestDataPersistor> _persistorInstanceSetter;
        private readonly Action<InMemorySqliteConfigurationParameters> _changeDbParams;

        public PersistenceTestConfiguration(AbstractTest test, Action<TestDataPersistor> persistorInstanceSetter, Action<InMemorySqliteConfigurationParameters> changeDbParams)
        {
            Condition.Requires(changeDbParams, "changeDbParams").IsNotNull();

            _test = test;
            _persistorInstanceSetter = persistorInstanceSetter;
            _changeDbParams = changeDbParams;
        }

        public IEnumerable<IConfigurationItem> GetItems(IDependencyAssembliesRegistry dependencyAssembliesRegistry)
        {
            var persistanceParameters = new InMemorySqliteConfigurationParameters
                                            {
                                                MappingAssemblies = dependencyAssembliesRegistry.MappingAssemblies,
                                                CacheProviderClass = typeof(HashtableCacheProvider),
                                            };
            _changeDbParams(persistanceParameters);
            yield return new InMemorySqlitePersistenceConfigurationItem(persistanceParameters);
        }

        public void AfterItemsApplying(IDependencyAssembliesRegistry dependencyAssembliesRegistry)
        {
            var persistor = new TestDataPersistor();
            _persistorInstanceSetter(persistor);

            var generationRules = new EnumEntitiesGenerationRules(_test.DataGenerator, persistor);
            var enumRulesScanner = new EnumGenerationRulesScanner(dependencyAssembliesRegistry.GenerationRulesAssemblies);
            enumRulesScanner.Scan();
            foreach (var generator in enumRulesScanner.Generators)
            {
                generator.Generate(generationRules);
            }
        }
    }
}