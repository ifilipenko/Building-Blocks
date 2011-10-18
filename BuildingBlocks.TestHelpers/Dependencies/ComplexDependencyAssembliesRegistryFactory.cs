using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Common.Logging;

namespace BuildingBlocks.TestHelpers.Dependencies
{
    class ComplexDependencyAssembliesRegistryFactory
    {
        private static readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public IDependencyAssembliesRegistry CreateForAssemblies(IEnumerable<Assembly> assemblies)
        {
            var dependencyAssembliesRegistryScanner = new DependencyAssembliesRegistryScanner(assemblies);
            dependencyAssembliesRegistryScanner.Scan();
            var registryTypes = dependencyAssembliesRegistryScanner.DependencyAssembliesRegistryTypes;
            if (registryTypes.Count() == 0)
            {
                throw new ConfigurationErrorsException("Not found implementations for " + typeof(IDependencyAssembliesRegistry));
            }
            _logger.Debug(m =>
                              {
                                  m("Found " + registryTypes.Count() + "-st dependency assemblies registries types" + Environment.NewLine);
                                  foreach (var type in registryTypes)
                                  {
                                      m(type + Environment.NewLine);
                                  }
                              });
            var registries = dependencyAssembliesRegistryScanner.DependencyAssembliesRegistryTypes
                .Select(t => (IDependencyAssembliesRegistry)Activator.CreateInstance(t))
                .ToList();
            return new ComplexDependencyAssembliesRegistry(registries);
        }
    }
}