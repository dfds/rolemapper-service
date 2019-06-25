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

        public DomainEventRegistry Register<TEvent>(string eventTypeName, string topicName, IEventHandler<TEvent> eventHandler)
        {
            throw new NotImplementedException();
        }

        public string GetTopicFor(string eventType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllTopics()
        {
            throw new NotImplementedException();
        }

        public Type GetInstanceTypeFor(string eventType)
        {
            throw new NotImplementedException();
        }

        public string GetTypeNameFor(IEvent domainEvent)
        {
            return _defaultTypeName;
        }

        public List<object> GetEventHandlersFor<TEvent>(TEvent domainEvent)
        {
            throw new NotImplementedException();
        }

        public DomainEventRegistrySpy(string defaultTypeName)
        {
            _defaultTypeName = defaultTypeName;
        }
    }
}