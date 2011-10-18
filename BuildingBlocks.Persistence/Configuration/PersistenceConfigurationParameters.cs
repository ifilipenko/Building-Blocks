using System;
using System.Reflection;
using BuildingBlocks.Persistence.Conventions.Configuration;
using BuildingBlocks.Persistence.Exports;

namespace BuildingBlocks.Persistence.Configuration
{
    public class PersistenceConfigurationParameters<TInheritor> 
        where TInheritor : PersistenceConfigurationParameters<TInheritor>, new()
    {
        public PersistenceConfigurationParameters()
        {
            SchemaUpdateMode = SchemaUpdateMode.Disabled;
            Conventions = new ConventionsConfiguration();
        }

        public ConventionsConfiguration Conventions { get; private set; }
        public Assembly[] MappingAssemblies { get; set; }
        public SchemaUpdateMode SchemaUpdateMode { get; set; }
        public Action<string> SchemaOutput { get; set; }
        public bool EnableStatistics { get; set; }
        public Type CacheProviderClass { get; set; }
        public bool EnableQueryLogging { get; set; }
        public IMappingExporter MappingExporter  { get; set; }

        public virtual TInheritor Clone()
        {
            var clone = new TInheritor
            {
                MappingExporter = MappingExporter == null ? null : MappingExporter.Clone(),
                MappingAssemblies = MappingAssemblies,
                SchemaUpdateMode = SchemaUpdateMode,
                SchemaOutput = SchemaOutput,
                EnableStatistics = EnableStatistics,
                CacheProviderClass = CacheProviderClass,
                EnableQueryLogging = EnableQueryLogging,
                Conventions = Conventions.Clone()
            };
            return clone;
        }
    }
}
