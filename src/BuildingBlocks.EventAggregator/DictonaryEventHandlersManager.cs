using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.EventAggregator
{
    public class DictionaryEventHandlersManager : IEventHandlersManager
    {
        readonly ConcurrentDictionary<Type, List<object>> _handlers = new ConcurrentDictionary<Type, List<object>>();

        public IEnumerable<IEventHandler<T>> GetHandlersOf<T>()
        {
            var handlers = _handlers.GetOrAdd(typeof(T), CreateDefaultHandlers);
            return handlers.OfType<IEventHandler<T>>().ToArray();
        }

        public void Remove<T>(IEventHandler<T> handler)
        {
            _handlers.AddOrUpdate(typeof(T),
                                  CreateDefaultHandlers,
                                  (type, list) =>
                                      {
                                          list.Remove(handler);
                                          return list;
                                      });
        }

        public void Add<T>(IEventHandler<T> handler)
        {
            _handlers.AddOrUpdate(typeof(T),
                                  CreateDefaultHandlers,
                                  (type, list) =>
                                      {
                                          if (!list.Any(l => l != null && l.GetType() == handler.GetType()))
                                          {
                                              list.Add(handler);
                                          }
                                          return list;
                                      });
        }

        private List<object> CreateDefaultHandlers(Type eventType)
        {
            return new List<object>(0);
        }
    }
}