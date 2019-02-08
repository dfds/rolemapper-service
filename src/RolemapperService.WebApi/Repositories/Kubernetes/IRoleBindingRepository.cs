using System.Threading.Tasks;

namespace RolemapperService.WebApi.Repositories.Kubernetes
{
    public interface IRoleBindingRepository
    {
        Task BindNamespaceRoleToGroup(string namespaceName, string role, string group);
    }
}