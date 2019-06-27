using System;
using Newtonsoft.Json.Linq;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class K8sNamespaceCreatedAndAwsArnConnectedEvent : IEvent
    {
        
        public string NamespaceName { get; }
        public Guid ContextId { get;  }

        public K8sNamespaceCreatedAndAwsArnConnectedEvent(string namespaceName, Guid contextId)
        {
            NamespaceName = namespaceName;
            ContextId = contextId;
        }
    }

    public class K8sNamespaceCreatedAndAwsArnConnectEventData
    {
        public string Description { get;  }
        
        public K8sNamespaceCreatedAndAwsArnConnectEventData(string description)
        {
            Description = description;
        }
    }
}