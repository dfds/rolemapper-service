using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace RolemapperService.WebApi.Repositories.Kubernetes
{
    public class NamespaceRespoitory : INamespaceRespoitory
    {
        private readonly IKubernetes _client;

        public NamespaceRespoitory(IKubernetes client)
        {
            _client = client;
        }

        public async Task CreateNamespace(string namespaceName, string roleName)
        {
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = namespaceName,
                    Annotations = new Dictionary<string, string>{{"iam.amazonaws.com/permitted",roleName}}
                }
            };
            
            var result = await _client.CreateNamespaceAsync(ns);
        }
    }
}