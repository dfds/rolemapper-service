using System.Collections.Generic;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public interface IPublishingEventsQueue
    {
        void AddEventToQueue(DomainEventEnvelope iEvent);
        void AddEventsToQueue(IEnumerable<DomainEventEnvelope> events);
        bool AnyEventInQueue();
        int QueueCount();
        DomainEventEnvelope Dequeue();
    }
}