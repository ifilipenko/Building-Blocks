using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BuildingBlocks.TestHelpers.Dependencies
{
    public class ComplexDependencyAssembliesRegistry : IDependencyAssembliesRegistry
    {
        private readonly IEnumerable<IDependencyAssembliesRegistry> _dependencyAssembliesRegistries;

        public ComplexDependencyAssembliesRegistry(IEnumerable<IDependencyAssembliesRegistry> dependencyAssembliesRegistries)
        {
            _dependencyAssembliesRegistries = dependencyAssembliesRegistries;
        }

        public Assembly[] MappingAssemblies
        {
            get { return _dependencyAssembliesRegistries.SelectMany(r => r.MappingAssemblies).ToArray(); }
        }

        public Assembly[] GenerationRulesAssemblies
        {
            get { return _dependencyAssembliesRegistries.SelectMany(r => r.GenerationRulesAssemblies).ToArray(); }
        }

        public Assembly[] AutomapperMapsAssemblies
        {
            get { return _dependencyAssembliesRegistries.SelectMany(r => r.AutomapperMapsAssemblies).ToArray(); }
        }
    }
}