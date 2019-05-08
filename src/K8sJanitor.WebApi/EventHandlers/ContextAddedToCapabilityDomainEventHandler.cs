using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;

namespace K8sJanitor.WebApi.EventHandlers
{
    public class ContextAddedToCapabilityDomainEventHandler : IEventHandler<ContextAddedToCapabilityDomainEvent>, WebApi.IEventHandler<ContextAddedToCapabilityDomainEvent>
    {
        private readonly INamespaceRepository _namespaceRepository;

        public ContextAddedToCapabilityDomainEventHandler(INamespaceRepository namespaceRepository)
        {
            _namespaceRepository = namespaceRepository;
        }

        public async Task HandleAsync(ContextAddedToCapabilityDomainEvent domainEvent)
        {
            var namespaceName = CreateNamespaceName(
                capabilityId: domainEvent.Data.CapabilityId,
                capabilityName: domainEvent.Data.CapabilityName,
                contextName: domainEvent.Data.ContextName
            );

            var labels = new Dictionary<string, string>
            {
                {"capability-id", domainEvent.Data.CapabilityId.ToString()},
                {"capability-name", domainEvent.Data.CapabilityName},
                {"context-id", domainEvent.Data.ContextId.ToString()},
                {"context-name", domainEvent.Data.ContextName}
            };

            await _namespaceRepository.CreateNamespaceAsync(namespaceName, labels);
        }

        public NamespaceName CreateNamespaceName(
            Guid capabilityId,
            string capabilityName,
            string contextName
        )
        {
            var uniqueCapability = capabilityName + "-" + capabilityId.ToString().Substring(0, 8);
 
            var name = uniqueCapability + "." + contextName;
            
            return NamespaceName.Create(name);
        }
    }
}