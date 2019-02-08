using System.Threading.Tasks;

namespace RolemapperService.WebApi.Repositories.Kubernetes
{
    public interface IRoleRepository
    {
        Task<string> CreateNamespaceFullAccessRole(string namespaceName);
    }
}