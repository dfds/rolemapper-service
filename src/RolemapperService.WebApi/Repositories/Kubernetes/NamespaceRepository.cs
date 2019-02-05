using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace RolemapperService.WebApi.Repositories.Kubernetes
{
    public class NamespaceRespoitory
    {
        private readonly IKubernetes _client;

        public NamespaceRespoitory(IKubernetes client)
        {
            _client = client;
        }

        public async Task CreateNamespace(string namespaceName)
        {
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = namespaceName
                }
            };
            
            var result = await _client.CreateNamespaceAsync(ns);
        }
    }
}