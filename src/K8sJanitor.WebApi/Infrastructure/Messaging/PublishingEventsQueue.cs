using System.Collections.Generic;
using System.Linq;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public class PublishingEventsQueue
    {
        private Queue<DomainEventEnvelope> _events;

        public PublishingEventsQueue()
        {
            _events = new Queue<DomainEventEnvelope>();
        }
        
        
        public void AddEventToQueue(DomainEventEnvelope iEvent)
        {
            _events.Enqueue(iEvent);
        }

        public void AddEventsToQueue(IEnumerable<DomainEventEnvelope> events)
        {
            foreach (DomainEventEnvelope @event in events)
            {
                _events.Enqueue(@event);
            }
        }
        
        public bool AnyEventInQueue()
        {
            return _events.Any();
        }

        public int QueueCount()
        {
            return _events.Count;
        }

        public DomainEventEnvelope Dequeue()
        {
            return _events.Dequeue();
        }

    }
}