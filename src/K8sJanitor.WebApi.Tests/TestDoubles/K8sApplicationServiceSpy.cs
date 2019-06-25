using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Application;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class K8sApplicationServiceSpy : IK8sApplicationService
    {
        public string Payload_Namespace { get; private set; }
        public Guid Payload_ContextId { get; private set; }
        public Guid Payload_CapabilityId { get; private set; }
        public Task FireEventK8sNamespaceCreatedAndAwsArnConnected(string namespaceName, Guid contextId, Guid capabilityId)
        {
            Payload_Namespace = namespaceName;
            Payload_ContextId = contextId;
            Payload_CapabilityId = capabilityId;
            return Task.CompletedTask;
        }
        
        
    }
}