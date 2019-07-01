using System;
using System.Threading.Tasks;

namespace K8sJanitor.WebApi.Application
{
    public interface IK8sApplicationService
    {
        Task FireEventK8sNamespaceCreatedAndAwsArnConnected(string namespaceName, Guid contextId);
    }
}