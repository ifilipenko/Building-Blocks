using System;
using System.Collections.Generic;
using BuildingBlocks.Configuration;
using BuildingBlocks.Configuration.Automapper;
using BuildingBlocks.TestHelpers.Dependencies;
using CuttingEdge.Conditions;

namespace BuildingBlocks.TestHelpers
{
    class AbstractTestConfiguration : ITestConfiguration
    {
        private readonly AbstractTest _abstractTest;
        private readonly Action<AutomapperMappingParameters> _changeAutomapperParameters;

        public AbstractTestConfiguration(AbstractTest abstractTest, Action<AutomapperMappingParameters> changeAutomapperParameters)
        {
            Condition.Requires(changeAutomapperParameters, "_changeAutomapperParameters").IsNotNull();

            _abstractTest = abstractTest;
            _changeAutomapperParameters = changeAutomapperParameters;
        }

        public IEnumerable<IConfigurationItem> GetItems(IDependencyAssembliesRegistry dependencyAssembliesRegistry)
        {
            var automapperConfigurationItem = GetAutomapperConfigurationItem(dependencyAssembliesRegistry);
            return new[] {automapperConfigurationItem};
        }

        public void AfterItemsApplying(IDependencyAssembliesRegistry dependencyAssembliesRegistry)
        {
            _abstractTest.DataGenerator = new DataGenerator.DataGenerator(dependencyAssembliesRegistry.GenerationRulesAssemblies);
        }

        protected virtual IConfigurationItem GetAutomapperConfigurationItem(IDependencyAssembliesRegistry dependencyAssembliesRegistry)
        {
            var automapperParameters = AutomapperMappingParameters
                .WithAssembliesFromArray(dependencyAssembliesRegistry.AutomapperMapsAssemblies);
            automapperParameters.ValidateMaps = true;
            _changeAutomapperParameters(automapperParameters);
            return new AutomapperMappingConfigurationItem(automapperParameters);
        }
    }
}