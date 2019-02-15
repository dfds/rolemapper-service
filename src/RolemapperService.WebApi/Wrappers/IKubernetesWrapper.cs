using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace RolemapperService.WebApi.Wrappers
{
    public interface IKubernetesWrapper
    {
        Task<V1Role> CreateNamespacedRoleAsync(V1Role body, string namespaceParameter, string pretty = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}