using System;

namespace K8sJanitor.WebApi.Repositories.Kubernetes
{
    public class KubernetesException : Exception
    {
        public KubernetesException(string message) : base(message)
        {
        }
    }

    public class NamespaceAlreadyExistException : KubernetesException
    {
        public NamespaceAlreadyExistException(string message) : base(message)
        {
        }
    }

    public class RoleAlreadyExistException : KubernetesException
    {
        public string RoleName { get; }

        public RoleAlreadyExistException(string message, string roleName) : base(message)
        {
            RoleName = roleName;
        }
    }

    public class AnnotationAlreadyExistWithDifferentValueException : KubernetesException
    {
        public AnnotationAlreadyExistWithDifferentValueException(string message) : base(message)
        {
            
        }
    }

    public class RoleBindingAlreadyExistInNamespaceException : KubernetesException
    {
        public string RoleBinding { get; }
        public string Namespace { get; }

        public RoleBindingAlreadyExistInNamespaceException(string message, string roleBindingName, string namespaceName) : base(message)
        {
            RoleBinding = roleBindingName;
            Namespace = namespaceName;
        }
    }

}