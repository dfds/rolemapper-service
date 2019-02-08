using System.Collections.Generic;
using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories.Kubernetes;

namespace RolemapperService.WebApi.Tests.TestDoubles
{
    public class NamespaceRepositorySpy : INamespaceRespoitory
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