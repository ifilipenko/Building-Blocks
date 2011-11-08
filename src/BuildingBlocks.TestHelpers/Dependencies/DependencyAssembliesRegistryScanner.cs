using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Configuration;

namespace BuildingBlocks.TestHelpers.Dependencies
{
    public class DependencyAssembliesRegistryScanner : AssemblyInterfaceImplementationScanner
    {
        private static readonly Type _dependencyAssembliesRegistry = typeof (IDependencyAssembliesRegistry);

        public DependencyAssembliesRegistryScanner(IEnumerable<Assembly> assemblies) 
            : base(assemblies)
        {
        }

        protected override bool ClassIsMatched(Type type)
        {
            return !type.IsGenericType;
        }

        protected override bool InterfaceIsMatched(Type interfaceType)
        {
            return interfaceType == _dependencyAssembliesRegistry;
        }

        public IEnumerable<Type> DependencyAssembliesRegistryTypes
        {
            get { return ScanProducts.Select(r => r.Type).ToList(); }
        }
    }
}