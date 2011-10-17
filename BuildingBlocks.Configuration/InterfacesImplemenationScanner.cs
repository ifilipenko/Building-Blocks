using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Common.Utils;
using Common.Logging;
using CuttingEdge.Conditions;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace BuildingBlocks.Configuration
{
    public class InterfacesImplemenationScanner
    {
        private static readonly ILog _log = LogManager.GetLogger<InterfacesImplemenationScanner>();
        private readonly ConfigurationExpression _iocContainerConfiguration;
        private readonly HashSet<Assembly> _interfacesAssemblies;
        private readonly Func<Type, bool> _interfaceFilter;

        public InterfacesImplemenationScanner(Action<InterfacesSelector> interfacesSelector, ConfigurationExpression iocContainerConfiguration)
        {
            _iocContainerConfiguration = iocContainerConfiguration;
            Condition.Requires(interfacesSelector, "interfacesSelector").IsNotNull();

            var selector = new InterfacesSelector();
            interfacesSelector(selector);

            _interfacesAssemblies = selector.AssembliesSet;
            _interfaceFilter = selector.InterfaceFilter;
        }

        public void FindImplementationsInAsseblies(Action<AssembliesListBuilder> assemliesListBuild)
        {
            Condition.Requires(assemliesListBuild, "assemliesListBuild").IsNotNull();

            var implAssembliesList = new List<Assembly>();
            assemliesListBuild(new AssembliesListBuilder(implAssembliesList));

            _log.Debug(m => m("Start scan assemblies for injection setup"));
            _iocContainerConfiguration
                .Scan(x =>
                {
                    foreach (var assembly in implAssembliesList)
                    {
                        x.Assembly(assembly);
                        _log.Debug(m => m("Add assembly to scanner {0}", assembly));
                    }

                    x.With(new InterfaceImplentationScannerConvention(_interfacesAssemblies, _interfaceFilter));
                });
        }

        class InterfaceImplentationScannerConvention : ConfigurableRegistrationConvention
        {
            private readonly HashSet<Assembly> _interfacesAssemblyList;
            private readonly Func<Type, bool> _interfaceFilter;

            public InterfaceImplentationScannerConvention(HashSet<Assembly> interfacesAssemblies, Func<Type, bool> interfaceFilter)
            {
                _interfacesAssemblyList = interfacesAssemblies;
                _interfaceFilter = interfaceFilter ?? NullInterfaceFilter;
            }

            public override void Process(Type concreteType, Registry registry)
            {
                if (!concreteType.IsConcrete() || !Constructor.HasConstructors(concreteType)) 
                    return;

                var pluginTypes = FindPluginType(concreteType);
                if (pluginTypes.IsNullOrEmpty()) 
                    return;

                foreach (var pluginType in pluginTypes)
                {
                    _log.Debug(m => m("Find plugin type \"{0}\" for concrete type \"{1}\"", pluginType, concreteType));
                    registry.For(pluginType).Use(concreteType);
                }
            }

            private IEnumerable<Type> FindPluginType(Type concreteType)
            {
                var ifaces = concreteType.GetInterfaces().Where(InterfaceIsMatched).ToArray();
                return ifaces;
            }

            private bool InterfaceIsMatched(Type ifaceType)
            {
                var containedInAssemblies = _interfacesAssemblyList.Count > 0 ? _interfacesAssemblyList.Contains(ifaceType.Assembly) : true;
                return containedInAssemblies && _interfaceFilter(ifaceType);
            }

            private bool NullInterfaceFilter(Type iface)
            {
                return true;
            }
        }
    }
}