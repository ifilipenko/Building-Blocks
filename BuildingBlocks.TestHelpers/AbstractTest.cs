using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Common.Sugar;
using BuildingBlocks.Configuration;
using BuildingBlocks.Configuration.Automapper;
using BuildingBlocks.TestHelpers.Dependencies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.TestHelpers
{
    [TestClass]
    public abstract class AbstractTest
    {
        public DataGenerator.DataGenerator DataGenerator { get; internal set; }

        [TestInitialize]
        public void SetUp()
        {
            var dependencyAssembliesRegistryFactory = new ComplexDependencyAssembliesRegistryFactory();
            var dependencyAssembliesRegistry = dependencyAssembliesRegistryFactory
                .CreateForAssemblies(DependencyRegistriesAssemblies);

            Configure(dependencyAssembliesRegistry);

            OnInitialized();
            before_each_tests();
        }

        private void Configure(IDependencyAssembliesRegistry dependencyAssembliesRegistry)
        {
            var abstractTestConfigurator = new AbstractTestConfiguration(this, ChangeAutomapperParameters);

            var configurations = GetConfigurations();
            configurations = configurations == null
                                ? new[] {abstractTestConfigurator}
                                : abstractTestConfigurator.ToEnumerable().Concat(configurations.Where(c => c != null));

            foreach (var configuration in configurations)
            {
                var configurationItems = configuration.GetItems(dependencyAssembliesRegistry);
                if (configurationItems != null)
                {
                    ApplyItems(configurationItems);
                    configuration.AfterItemsApplying(dependencyAssembliesRegistry);
                }
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            after_each_tests();
            OnFinalized();
        }

        protected virtual void OnInitialized()
        {
        }

        protected virtual void OnFinalized()
        {
        }

        protected abstract void before_each_tests();
        protected abstract void after_each_tests();

        protected virtual IEnumerable<ITestConfiguration> GetConfigurations()
        {
            return Enumerable.Empty<ITestConfiguration>();
        }

        protected virtual IEnumerable<Assembly> DependencyRegistriesAssemblies
        {
            get { yield return GetType().Assembly; }
        }

        protected virtual void ChangeAutomapperParameters(AutomapperMappingParameters automapperParameters)
        {
        }

        private static void ApplyItems(IEnumerable<IConfigurationItem> configurationItems)
        {
            var configurator = new Configurator();
            foreach (var configurationItem in configurationItems.Where(i => i != null))
            {
                configurator.AddItem(configurationItem);
            }
            configurator.Configure();
        }
    }
}