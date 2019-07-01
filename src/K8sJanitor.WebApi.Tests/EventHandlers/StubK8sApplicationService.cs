using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Application;

namespace K8sJanitor.WebApi.Tests.EventHandlers
{
    public class StubK8sApplicationService : IK8sApplicationService
    {
        public Task FireEventK8sNamespaceCreatedAndAwsArnConnected(string namespaceName, Guid contextId)
        {
            return Task.CompletedTask;
        }
    }

    public class ErroneousK8sApplicationService : IK8sApplicationService
    {
        private readonly Exception _exceptionToThrow;

        public ErroneousK8sApplicationService(Exception exceptionToThrow)
        {
            _exceptionToThrow = exceptionToThrow;
        }
        
        public Task FireEventK8sNamespaceCreatedAndAwsArnConnected(string namespaceName, Guid contextId)
        {
            throw _exceptionToThrow;
        }
    }
}