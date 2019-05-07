using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;

namespace K8sJanitor.WebApi.EventHandlers
{
    public class ContextAddedToCapabilityDomainEventHandler : IEventHandler<ContextAddedToCapabilityDomainEvent>, WebApi.IEventHandler<ContextAddedToCapabilityDomainEvent>
    {
        public Task HandleAsync(ContextAddedToCapabilityDomainEvent domainEvent)
        {
            Console.WriteLine($"ContextId: \"{domainEvent.Data.ContextId}\" ContextName: \"{domainEvent.Data.ContextName}\" is added to CapabilityId: \"{domainEvent.Data.CapabilityId}\" CapabilityName: \"{domainEvent.Data.CapabilityName}\"");
        
            return Task.CompletedTask;
        }
    }
}