using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace K8sJanitor.WebApi.Wrappers
{
    public class KubernetesWrapper : IKubernetesWrapper
    {
        private readonly IKubernetes _kubernetes;

        public KubernetesWrapper(IKubernetes kubernetes)
        {
            _kubernetes = kubernetes;
        }


        public Task<V1Role> CreateNamespacedRoleAsync(V1Role body, string namespaceParameter, string pretty = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _kubernetes.CreateNamespacedRoleAsync(body, namespaceParameter, pretty, cancellationToken);
        }
    }
}