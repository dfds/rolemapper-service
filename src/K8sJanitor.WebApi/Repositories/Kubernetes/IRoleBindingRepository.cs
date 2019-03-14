using System.Threading.Tasks;

namespace K8sJanitor.WebApi.Repositories.Kubernetes
{
    public interface IRoleBindingRepository
    {
        Task BindNamespaceRoleToGroup(string namespaceName, string role, string group);
    }
}