using System.Collections.Generic;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Models;

namespace K8sJanitor.WebApi.Repositories.Kubernetes
{
    public interface INamespaceRepository
    {
        Task CreateNamespaceAsync(string namespaceName, string roleName);

        Task CreateNamespaceAsync(
            NamespaceName namespaceName,
            IEnumerable<Label> labels
        );
        Task CreateNamespaceAsync(NamespaceName namespaceName);

        Task AddAnnotations(NamespaceName namespaceName, Dictionary<string, string> annotations);

    }
}