using System;

namespace K8sJanitor.WebApi.Repositories.Kubernetes
{
    public class KubernetesException : Exception
    {
        public KubernetesException(string message) : base(message)
        {
        }
    }
    
    public class NamespaceAlreadyExistException: KubernetesException
    {
        public NamespaceAlreadyExistException(string message) : base(message)
        {
        }
    }
}