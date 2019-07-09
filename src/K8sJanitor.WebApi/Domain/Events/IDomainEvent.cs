using System;

namespace K8sJanitor.WebApi.Domain.Events
{
    public interface IDomainEvent<T>
    {
        string Version { get; }
        string EventName { get; }
        string XCorrelationId { get; }
        string XSender { get; }
        T Payload { get; }
    }
}