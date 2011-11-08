using System;
using System.Collections.Generic;
using System.Reflection;

namespace BuildingBlocks.Configuration.Automapper
{
    class FindAutomapperMaps : AssemblyInterfaceImplementationScanner
    {
        private static readonly Type _automapperMapsType = typeof(IAutomapperMaps<>);

        public FindAutomapperMaps(IEnumerable<Assembly> assemblies) 
            : base(assemblies)
        {
        }

        public IEnumerable<InterfacesImplemenation> FoundedAutomapperMaps 
        {
            get { return ScanProducts; }
        }

        protected override bool ClassIsMatched(Type type)
        {
            return !type.IsGenericType;
        }

        protected override bool InterfaceIsMatched(Type interfaceType)
        {
            return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _automapperMapsType;
        }
    }
}