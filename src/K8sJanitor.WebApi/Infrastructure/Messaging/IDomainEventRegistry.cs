using System;
using System.Collections.Generic;
using K8sJanitor.WebApi.Domain.Events;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public interface IDomainEventRegistry : IDomainEventRegistrationManager
    {
        string GetTopicFor(string eventType);
        string GetTypeNameFor(IDomainEvent<object> domainEvent); // IDomainEvent is different from the implementation in CapabilityService, ensure this is the right approach
        IEnumerable<string> GetAllTopics();
        Type GetInstanceTypeFor(string eventName);
    }
}