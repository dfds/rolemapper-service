using System.Threading.Tasks;

namespace RolemapperService.WebApi.Repositories.Kubernetes
{
    public interface INamespaceRespoitory
    {
        Task CreateNamespace(string namespaceName, string roleName);
    }
}