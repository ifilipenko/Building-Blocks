using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Configuration;

namespace BuildingBlocks.Persistence.TestHelpers.TestData
{
    internal class EnumGenerationRulesScanner : AssemblyInterfaceImplementationScanner
    {
        public EnumGenerationRulesScanner(IEnumerable<Assembly> assemblies) 
            : base(assemblies)
        {
        }

        public IEnumerable<IEnumEntitiesGenerator> Generators
        {
            get
            {
                return ScanProducts
                    .Select(x => (IEnumEntitiesGenerator) Activator.CreateInstance(x.Type))
                    .ToList();
            }
        }

        protected override bool ClassIsMatched(Type type)
        {
            return !type.IsGenericType;
        }

        protected override bool InterfaceIsMatched(Type interfaceType)
        {
            return interfaceType == typeof (IEnumEntitiesGenerator);
        }
    }
}