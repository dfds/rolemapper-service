using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Application;

namespace K8sJanitor.WebApi.EventHandlers
{
    public class StubK8sApplicationService : IK8sApplicationService
    {
        public Task TestCreated(string description)
        {
            throw new NotImplementedException();
        }

        public Task FireEventK8sNamespaceCreatedAndAwsArnConnected(string namespaceName, Guid contextId)
        {
            throw new NotImplementedException();
        }
    }
}