using System.Collections.Generic;

namespace BuildingBlocks.EventAggregator
{
    public interface IEventHandlersManager
    {
        IEnumerable<IEventHandler<T>> GetHandlersOf<T>();
        void Remove<T>(IEventHandler<T> handler);
        void Add<T>(IEventHandler<T> handler);
    }
}