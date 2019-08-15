using System.Collections.Generic;
using K8sJanitor.WebApi.Infrastructure.Messaging;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class PublishingEventsQueueSpy : IPublishingEventsQueue
    {
        public Queue<DomainEventEnvelope> Queue;
        public void AddEventToQueue(DomainEventEnvelope iEvent)
        {
            Queue.Enqueue(iEvent);
        }

        public void AddEventsToQueue(IEnumerable<DomainEventEnvelope> events)
        {
            throw new System.NotImplementedException();
        }

        public bool AnyEventInQueue()
        {
            throw new System.NotImplementedException();
        }

        public int QueueCount()
        {
            throw new System.NotImplementedException();
        }

        public DomainEventEnvelope Dequeue()
        {
            throw new System.NotImplementedException();
        }

        public PublishingEventsQueueSpy()
        {
            Queue = new Queue<DomainEventEnvelope>();
        }
    }
}