using System;
using System.Collections.Generic;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public interface IDomainEventRegistry
    {
        IDomainEventRegistry Register<TEvent>(string eventTypeName, string topicName) where TEvent : IEvent;

        string GetTopicFor(string eventType);

        IEnumerable<string> GetAllTopics();
        
        Type GetTypeFor(string eventType);

        string GetTypeNameFor(IEvent @event);

        IEnumerable<IEventHandler> GetEventHandlersFor(IEvent @event);
    }
}