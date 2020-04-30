using System;

namespace K8sJanitor.WebApi.Domain.Events
{
    public interface IDomainEvent<out T> : IEvent
    {
        string Version { get; }

        string EventName { get; }

        string XCorrelationId { get; }

        string XSender { get; }

        T Payload { get; }
    }
}