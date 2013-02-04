using System.Collections;
using System.Collections.Generic;

namespace BuildingBlocks.EventAggregator
{
    public interface IEventHandler<in TEvent>
    {
        void Handle(TEvent @event);
    }
}