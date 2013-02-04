namespace BuildingBlocks.EventAggregator
{
    public class SimpleEventPublisher : IEventPublisher
    {
        private readonly IEventHandlersManager _eventHandlersManager;

        public SimpleEventPublisher(IEventHandlersManager eventHandlersManager)
        {
            _eventHandlersManager = eventHandlersManager;
        }

        public void Publish<TEvent>(TEvent @event)
        {
            foreach (var eventHandler in _eventHandlersManager.GetHandlersOf<TEvent>())
            {
                eventHandler.Handle(@event);
            }
        }
    }
}