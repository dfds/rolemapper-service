using System.Collections.Generic;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class NamespaceRepositorySpy : INamespaceRepository
    {
        public List<KeyValuePair<string, string>> Namespaces { get; }

        public NamespaceRepositorySpy()
        {
            Namespaces = new List<KeyValuePair<string, string>>();
        }
            
        public Task CreateNamespaceAsync(string namespaceName, string roleName)
        {
            Namespaces.Add(new KeyValuePair<string, string>(namespaceName, roleName));

            return Task.CompletedTask;
        }

        public Task CreateNamespaceAsync(NamespaceName namespaceName, IDictionary<string, string> labels)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateNamespaceAsync(NamespaceName namespaceName)
        {
            throw new System.NotImplementedException();
        }

        public Task AddAnnotations(NamespaceName namespaceName, Dictionary<string, string> annotations)
        {
            throw new System.NotImplementedException();
        }
    }
}