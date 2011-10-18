using System.Collections.Generic;
using System.Reflection;
using BuildingBlocks.Configuration;

namespace BuildingBlocks.TestHelpers.Dependencies
{
    public abstract class AbstractDependencyAssembliesRegistry : IDependencyAssembliesRegistry
    {
        private readonly List<Assembly> _automapperMapsAssemblies;
        private readonly List<Assembly> _mappingAssemblies;
        private readonly List<Assembly> _generationRulesAssemblies;

        public AbstractDependencyAssembliesRegistry()
        {
            _automapperMapsAssemblies = new List<Assembly>();
            _mappingAssemblies = new List<Assembly>();
            _generationRulesAssemblies = new List<Assembly>();

            SetMappingAssemblies(new AssembliesListBuilder(_mappingAssemblies));
            SetAutomapperMapsAssemblies(new AssembliesListBuilder(_automapperMapsAssemblies));
            SetGenerationRulesAssemblies(new AssembliesListBuilder(_generationRulesAssemblies));
        }

        protected abstract void SetMappingAssemblies(AssembliesListBuilder assembliesListBuilder);
        protected abstract void SetAutomapperMapsAssemblies(AssembliesListBuilder assembliesListBuilder);
        protected abstract void SetGenerationRulesAssemblies(AssembliesListBuilder assembliesListBuilder);

        public Assembly[] MappingAssemblies
        {
            get { return _mappingAssemblies.ToArray(); }
        }

        public Assembly[] GenerationRulesAssemblies
        {
            get { return _generationRulesAssemblies.ToArray(); }
        }

        public Assembly[] AutomapperMapsAssemblies
        {
            get { return _automapperMapsAssemblies.ToArray(); }
        }
    }
}