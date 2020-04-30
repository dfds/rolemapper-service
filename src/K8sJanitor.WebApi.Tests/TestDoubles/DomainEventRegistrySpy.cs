using System;
using System.Collections.Generic;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Infrastructure.Messaging;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class DomainEventRegistrySpy : IDomainEventRegistry
    {
        private readonly string _defaultTypeName;
        
        public string GetTopicFor(string eventType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllTopics()
        {
            throw new NotImplementedException();
        }

        public string GetTypeNameFor(IEvent domainEvent)
        {
            return _defaultTypeName;
        }

        public IDomainEventRegistry Register<TEvent>(string eventTypeName, string topicName) where TEvent : IEvent
        {
            throw new NotImplementedException();
        }

        public Type GetTypeFor(string eventType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEventHandler> GetEventHandlersFor(IEvent @event)
        {
            throw new NotImplementedException();
        }

        public DomainEventRegistrySpy(string defaultTypeName)
        {
            _defaultTypeName = defaultTypeName;
        }
    }
}