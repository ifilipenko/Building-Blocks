using System;

namespace BuildingBlocks.EventAggregator
{
    public static class ReactiveEventSubscriberHelper
    {
        public static IDisposable SubscribeOn<TEvent>(this IReactiveEventSubscriber subscriber, Action<TEvent> onEvent)
        {
            return subscriber.GetEvent<TEvent>().Subscribe(onEvent);
        }
    }
}