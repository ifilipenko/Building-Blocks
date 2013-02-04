namespace BuildingBlocks.EventAggregator
{
    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent @event);
    }
}