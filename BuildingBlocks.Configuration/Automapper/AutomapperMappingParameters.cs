using System;
using System.Reflection;
using BuildingBlocks.Configuration.AssemblyLoad;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Configuration.Automapper
{
    public class AutomapperMappingParameters
    {
        public static AutomapperMappingParameters WithAssembliesFromArray(Assembly[] assemblies)
        {
            var parameters = new AutomapperMappingParameters { _mappingAssemblies = assemblies };
            parameters._mappingAssembliesGetter = parameters.GetMappingAssembliesFromField;
            return parameters;
        }

        public static AutomapperMappingParameters WithAssembliesFromDirectory(string filePath)
        {
            return WithAssembliesFromFiles(filePath, f => f.EndsWith(".dll"));
        }

        public static AutomapperMappingParameters WithAssembliesLoader(IAssembliesLoader assembliesLoader)
        {
            Condition.Requires(assembliesLoader, "assembliesLoader").IsNotNull();

            var parameters = new AutomapperMappingParameters
                                 {
                                     _assembliesLoader = assembliesLoader
                                 };
            parameters._mappingAssembliesGetter = parameters.GetMappingAssembliesFromAssembliesLoader;
            return parameters;
        }

        public static AutomapperMappingParameters WithAssembliesFromFiles(string filePath, Func<string, bool> fileNameCondition)
        {
            var parameters = new AutomapperMappingParameters
                                 {
                                     _assembliesLoader = new FileSystemAssembliesLoader(filePath, fileNameCondition)
                                 };
            parameters._mappingAssembliesGetter = parameters.GetMappingAssembliesFromAssembliesLoader;
            return parameters;
        }

        private Assembly[] _mappingAssemblies;
        private Func<Assembly[]> _mappingAssembliesGetter;
        private IAssembliesLoader _assembliesLoader;

        AutomapperMappingParameters()
        {
        }
        
        public Assembly[] MappingAssemblies 
        {
            get { return _mappingAssembliesGetter(); }
        }

        public bool ValidateMaps { get; set; }

        private Assembly[] GetMappingAssembliesFromField()
        {
            return _mappingAssemblies;
        }

        private Assembly[] GetMappingAssembliesFromAssembliesLoader()
        {
            return _assembliesLoader.LoadAssemblies();
        }
    }
}