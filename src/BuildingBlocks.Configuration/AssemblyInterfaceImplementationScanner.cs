using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Configuration
{
    public abstract class AssemblyInterfaceImplementationScanner
    {
        private readonly IEnumerable<Assembly> _assemblies;
        private List<InterfacesImplemenation> _scanProducts;

        protected AssemblyInterfaceImplementationScanner(IEnumerable<Assembly> assemblies)
        {
            Condition.Requires(assemblies, "assemblies").IsNotNull();
            _assemblies = assemblies;
        }

        public void Scan()
        {
            var types = _assemblies.SelectMany(t => t.GetTypes());
            _scanProducts = (from type in types
                             where !type.IsInterface && ClassIsMatched(type)
                             let interfaces = (from i in type.GetInterfaces()
                                               where InterfaceIsMatched(i)
                                               select i).ToList()
                             where interfaces.Count != 0
                             select new InterfacesImplemenation(type, interfaces)).ToList();
        }

        protected abstract bool ClassIsMatched(Type type);
        protected abstract bool InterfaceIsMatched(Type interfaceType);

        protected IEnumerable<InterfacesImplemenation> ScanProducts
        {
            get { return _scanProducts; }
        }
    }
}