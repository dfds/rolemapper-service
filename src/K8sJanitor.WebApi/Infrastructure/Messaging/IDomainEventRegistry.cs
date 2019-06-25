using System;
using System.Collections.Generic;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public interface IDomainEventRegistry
    {
        DomainEventRegistry Register<TEvent>(string eventTypeName, string topicName, IEventHandler<TEvent> eventHandler);
        string GetTopicFor(string eventType);
        IEnumerable<string> GetAllTopics();
        Type GetInstanceTypeFor(string eventType);
        string GetTypeNameFor(IEvent domainEvent);
        List<object> GetEventHandlersFor<TEvent>(TEvent domainEvent);
    }
}