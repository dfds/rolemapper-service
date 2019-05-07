using System.Threading.Tasks;

namespace K8sJanitor.WebApi.Repositories.Kubernetes
{
    public interface INamespaceRepository
    {
        Task CreateNamespace(string namespaceName, string roleName);
    }
}