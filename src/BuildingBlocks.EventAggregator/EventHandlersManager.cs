using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.EventAggregator
{
    class EventHandlersManager : IEventHandlersManager
    {
        private readonly ConcurrentDictionary<Type, List<object>> _handlers = new ConcurrentDictionary<Type, List<object>>();

        public IEnumerable<IEventHandler<TEvent>> GetHandlersOf<TEvent>()
        {
            return _handlers.OfType<IEventHandler<TEvent>>();
        }

        public void Remove<TEvent>(IEventHandler<TEvent> handler)
        {
            List<object> eventHandlers;
            if (!_handlers.TryGetValue(typeof (TEvent), out eventHandlers) || eventHandlers == null)
                return;

            eventHandlers.Remove(handler);
            _handlers.GetOrAdd(typeof(TEvent), eventHandlers);
        }

        public void Add<TEvent>(IEventHandler<TEvent> handler)
        {
            if (ContainsHandler(handler))
                return;

            List<object> handlers;
            if (!_handlers.TryGetValue(typeof (TEvent), out handlers)) 
                return;

            if (handlers == null)
            {
                handlers = new List<object>();
            }
            handlers.Add(handler);
            _handlers.GetOrAdd(typeof (TEvent), handlers);
        }

        private bool ContainsHandler<TEvent>(IEventHandler<TEvent> handler)
        {
            List<object> eventHandlers;
            return _handlers.TryGetValue(typeof (TEvent), out eventHandlers) && (eventHandlers != null && eventHandlers.Contains(handler));
        }
    }
}