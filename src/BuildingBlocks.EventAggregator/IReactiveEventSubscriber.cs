using System;

namespace BuildingBlocks.EventAggregator
{
    public interface IReactiveEventSubscriber
    {
        IObservable<TEvent> GetEvent<TEvent>();
    }
}