using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Rest;

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

        public Task CreateNamespacedConfigMapAsync(V1ConfigMap body, string namespaceParameter)
        {
            return _kubernetes.CreateNamespacedConfigMapAsync(body, namespaceParameter);
        }

        public Task ReplaceNamespacedConfigMapAsync(V1ConfigMap body, string name, string namespaceParameter)
        {
            return _kubernetes.ReplaceNamespacedConfigMapAsync(body, name, namespaceParameter);
        }

        public async Task<V1ConfigMap> ReadNamespacedConfigMapAsync(
            string name,
            string namespaceParameter,
            bool? exact = null,
            bool? export = null,
            string pretty = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _kubernetes.ReadNamespacedConfigMapAsync(
                name,
                namespaceParameter,
                exact,
                export,
                pretty,
                cancellationToken
            );
        }

        public async Task<V1Namespace> ReadNamespaceAsync(
            string name,
            bool? exact = null,
            bool? export = null,
            string pretty = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _kubernetes.ReadNamespaceAsync(
                name,
                exact,
                export,
                pretty,
                cancellationToken
            );
        }


        public async Task<HttpOperationResponse<V1Namespace>> PatchNamespaceWithHttpMessagesAsync(
            V1Patch body,
            string name,
            string pretty = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _kubernetes.PatchNamespaceWithHttpMessagesAsync(
                body,
                name,
                pretty,
                customHeaders,
                cancellationToken
            );
        }

        public Task CreateNamespaceAsync(
            V1Namespace body, 
            string pretty, 
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            return _kubernetes.CreateNamespaceAsync(
                body, 
                pretty, 
                cancellationToken
            );
        }

        public Task<V1RoleBinding> CreateNamespacedRoleBindingAsync(
            V1RoleBinding body, 
            string namespaceParameter, 
            string pretty = null,
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            return _kubernetes.CreateNamespacedRoleBindingAsync(
                body,
                namespaceParameter,
                pretty,
                cancellationToken
            );
        }

        public async Task<IEnumerable<V1Namespace>> GetAllCapabilityNamespacesAsync()
        {
            var namespaces = await _kubernetes.ListNamespaceAsync();

            var capabilityNamespaces = namespaces.Items
                .Where(i => 
                    i.Metadata?.Labels?
                        .Any(l => 
                            l.Key == "capability-id"
                        ) == true
                );
            return capabilityNamespaces;
        }
    }
}