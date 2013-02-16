using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Common;

namespace BuildingBlocks.EventAggregator
{
    public class EventHandlersRegistrar
    {
        private readonly IIocContainer _container;

        public EventHandlersRegistrar(IIocContainer container)
        {
            _container = container;
        }

        public void RegisterToManager(IEventHandlersManager eventHandlersManager, IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException("assemblies");
            if (eventHandlersManager == null)
                throw new ArgumentNullException("eventHandlersManager");

            var publicTypes = assemblies.SelectMany(a => a.GetTypes())
                                        .Where(t => t.IsPublic && !t.IsAbstract && t.IsClass);

            foreach (var type in publicTypes)
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;
                RegisterHandler(type, eventHandlersManager);
            }
        }

        private void RegisterHandler(Type type, IEventHandlersManager eventHandlersManager)
        {
            var interfaces = (from iface in type.GetInterfaces()
                              where iface.IsGenericType &&
                                    iface.GetGenericTypeDefinition() == typeof (IEventHandler<>)
                              select iface).ToArray();

            if (interfaces.Length == 0)
                return;

            var addHandlerGenericMethod = eventHandlersManager.GetType().GetMethod("Add");
            var handler = _container.Resolve(type);
            foreach (var @interface in interfaces)
            {
                var addHandlerMethod = addHandlerGenericMethod.MakeGenericMethod(@interface.GetGenericArguments());
                addHandlerMethod.Invoke(eventHandlersManager, new[] {handler});
            }
        }
    }
}