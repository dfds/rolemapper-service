using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.Models;

namespace K8sJanitor.WebApi.EventHandlers
{
    public class ContextAddedToCapabilityDomainEventHandler : IEventHandler<ContextAddedToCapabilityDomainEvent>, WebApi.IEventHandler<ContextAddedToCapabilityDomainEvent>
    {
        public Task HandleAsync(ContextAddedToCapabilityDomainEvent domainEvent)
        {
            Console.WriteLine($"ContextId: \"{domainEvent.Data.ContextId}\" ContextName: \"{domainEvent.Data.ContextName}\" is added to CapabilityId: \"{domainEvent.Data.CapabilityId}\" CapabilityName: \"{domainEvent.Data.CapabilityName}\"");

            var namespaceName = CreateNamespaceName(
                capabilityId: domainEvent.Data.CapabilityId,
                capabilityName: domainEvent.Data.CapabilityName,
                contextName: domainEvent.Data.ContextName
            );
            Console.WriteLine($"Name will be: \"{namespaceName}\"");
            return Task.CompletedTask;
        }

        public NamespaceName CreateNamespaceName(
            Guid capabilityId,
            string capabilityName,
            string contextName
        )
        {
            var uniqueContext = capabilityName + "-" + capabilityId.ToString().Substring(0, 8);

            var name = uniqueContext + "." + contextName;
            
            return NamespaceName.Create(name);
        }
    }
}