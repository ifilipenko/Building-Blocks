using System;

namespace BuildingBlocks.EventAggregator.Rx
{
    public interface IReactiveEventSubscriber
    {
        IObservable<TEvent> GetEvent<TEvent>();
    }
}