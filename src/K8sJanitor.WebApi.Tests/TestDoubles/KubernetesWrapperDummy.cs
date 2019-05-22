using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using K8sJanitor.WebApi.Wrappers;
using Microsoft.Rest;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class KubernetesWrapperDummy : IKubernetesWrapper
    {
        public Task<V1Role> CreateNamespacedRoleAsync(V1Role body, string namespaceParameter, string pretty = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(body);
        }

        public Task CreateNamespacedConfigMapAsync(V1ConfigMap body, string namespaceParameter)
        {
            return Task.CompletedTask;
        }

        public Task ReplaceNamespacedConfigMapAsync(V1ConfigMap body, string name, string namespaceParameter)
        {
            return Task.CompletedTask;
        }

        public Task<V1ConfigMap> ReadNamespacedConfigMapAsync(string name, string namespaceParameter, bool? exact = null, bool? export = null,
            string pretty = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new V1ConfigMap());
        }

        public Task<V1Namespace> ReadNamespaceAsync(string name, bool? exact = null, bool? export = null, string pretty = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new V1Namespace());
        }

        public Task<HttpOperationResponse<V1Namespace>> PatchNamespaceWithHttpMessagesAsync(V1Patch body, string name, string pretty = null,
            Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new HttpOperationResponse<V1Namespace>());
        }

        public Task CreateNamespaceAsync(V1Namespace body, string pretty = null, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<V1RoleBinding> CreateNamespacedRoleBindingAsync(V1RoleBinding body, string namespaceParameter, string pretty = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new V1RoleBinding());
        }
    }
}