using System;
using Newtonsoft.Json.Linq;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class K8sNamespaceCreatedAndAwsArnConnectedEvent : IEvent
    {
        
        public string NamespaceName { get; }
        public Guid ContextId { get;  }
        public Guid CapabilityId { get;  }

        public K8sNamespaceCreatedAndAwsArnConnectedEvent(string namespaceName, Guid contextId, Guid capabilityId)
        {
            NamespaceName = namespaceName;
            ContextId = contextId;
            CapabilityId = capabilityId;
        }

        public K8sNamespaceCreatedAndAwsArnConnectedEvent()
        {
            
        }

        public K8sNamespaceCreatedAndAwsArnConnectedEvent(GeneralDomainEvent domainEvent)
        {
            
        }
    }
}