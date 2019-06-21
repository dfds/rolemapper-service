using System;
using System.Collections.Generic;
using K8sJanitor.WebApi.Domain.Events;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public interface IDomainEventRegistrationManager
    {
        IEnumerable<DomainEventRegistry.DomainEventRegistration> Registrations { get;  }
        IDomainEventRegistrationManager Register<TEvent>(string eventTypeName, string topicName) where TEvent : IEvent;
        bool IsRegistered(Type eventInstanceType);
    }
}