using System.Collections.Generic;
using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories.Kubernetes;

namespace RolemapperService.WebApi.Tests.TestDoubles
{
    public class RoleRepositorySpy : IRoleRepository
    {
        public List<string> Namespaces { get;}

        public RoleRepositorySpy()
        {
            Namespaces = new List<string>();
        }
        
        public Task<string> CreateNamespaceFullAccessRole(string namespaceName)
        {
            Namespaces.Add(namespaceName);

            return Task.FromResult(namespaceName + "-full-access-role");
        }
    }
}