using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using K8sJanitor.WebApi.Models;
using Microsoft.Rest;

namespace K8sJanitor.WebApi.Wrappers
{
    public interface IKubernetesWrapper
    {
        Task<V1Role> CreateNamespacedRoleAsync(V1Role body, string namespaceParameter, string pretty = null, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateNamespacedConfigMapAsync(V1ConfigMap body, string namespaceParameter);
        Task ReplaceNamespacedConfigMapAsync(V1ConfigMap body, string name, string namespaceParameter);
        Task<V1ConfigMap> ReadNamespacedConfigMapAsync(
            string name,
            string namespaceParameter,
            bool? exact = null,
            bool? export = null,
            string pretty = null,
            CancellationToken cancellationToken= default(CancellationToken)
        );
        
        Task<V1Namespace> ReadNamespaceAsync(
            string name,
            bool? exact = null,
            bool? export = null,
            string pretty = null,
            CancellationToken cancellationToken= default(CancellationToken)
        );

        Task<HttpOperationResponse<V1Namespace>> PatchNamespaceWithHttpMessagesAsync(
            V1Patch body,
            string name,
            string pretty = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken)
        );

        Task CreateNamespaceAsync(
            V1Namespace body,
            string pretty = null,
            CancellationToken cancellationToken = default(CancellationToken)
        );

        Task<V1RoleBinding> CreateNamespacedRoleBindingAsync(
            V1RoleBinding body,
            string namespaceParameter,
            string pretty = null,
            CancellationToken cancellationToken = default(CancellationToken)
        );
    }
}