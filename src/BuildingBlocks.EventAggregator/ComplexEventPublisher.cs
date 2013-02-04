namespace BuildingBlocks.EventAggregator
{
    public class ComplexEventPublisher : IEventPublisher
    {
        private readonly IEventPublisher[] _eventAggregators;

        public ComplexEventPublisher(params IEventPublisher[] eventAggregators)
        {
            _eventAggregators = eventAggregators;
        }

        public void Publish<TEvent>(TEvent @event)
        {
            foreach (var eventAggregator in _eventAggregators)
            {
                eventAggregator.Publish(@event);
            }
        }
    }
}