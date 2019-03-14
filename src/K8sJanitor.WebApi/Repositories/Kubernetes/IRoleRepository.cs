using System.Threading.Tasks;

namespace K8sJanitor.WebApi.Repositories.Kubernetes
{
    public interface IRoleRepository
    {
        Task<string> CreateNamespaceFullAccessRole(string namespaceName);
    }
}