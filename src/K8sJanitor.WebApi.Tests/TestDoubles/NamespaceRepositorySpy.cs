using System.Collections.Generic;
using System.Threading.Tasks;
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
            
        public Task CreateNamespace(string namespaceName, string roleName)
        {
            Namespaces.Add(new KeyValuePair<string, string>(namespaceName, roleName));

            return Task.CompletedTask;
        }
    }
}