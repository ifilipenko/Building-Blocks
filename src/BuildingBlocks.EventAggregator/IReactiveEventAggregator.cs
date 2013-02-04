namespace BuildingBlocks.EventAggregator
{
    public interface IReactiveEventAggregator : IEventPublisher, IReactiveEventSubscriber
    {
    }
}