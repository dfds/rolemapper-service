using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace RolemapperService.WebApi.Repositories
{
    public interface IKubernetesRepository
    {
        Task<string> GetAwsAuthConfigMap();
        Task<string> GetAwsAuthConfigMapRoleMap();
        Task<string> ReplaceAwsAuthConfigMapRoleMap(string configMapRoleMap);
        Task<string> PatchAwsAuthConfigMapRoleMap(string configMapRoleMap);
    }
}