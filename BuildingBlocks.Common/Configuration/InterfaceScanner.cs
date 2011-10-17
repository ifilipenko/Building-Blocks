using System;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace BuildingBlocks.Common.Configuration
{
    public class InterfaceScanner<TInterface> : IRegistrationConvention
    {
        public InterfaceScanner()
        {
            if (!typeof(TInterface).IsInterface)
            {
                throw new ArgumentException("Type is not interface");
            }
        }

        public void Process(Type type, Registry registry)
        {
            if (type.IsInterface || type.IsAbstract)
                return;

            var ifaces = type.GetInterfaces();
            var targetInterface = typeof(TInterface);
            if (ifaces.Any(i => i == targetInterface))
            {
                var interfaceType = (from i in ifaces
                                     where i.GetInterface(targetInterface.Name) != null
                                     select i).FirstOrDefault();
                if (interfaceType == null)
                    return;
                registry.For(interfaceType).Use(type);
            }
        }
    }
}