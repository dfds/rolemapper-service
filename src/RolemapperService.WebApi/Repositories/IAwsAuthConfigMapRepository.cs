using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace RolemapperService.WebApi.Repositories
{
    public interface IAwsAuthConfigMapRepository
    {
        Task<string> GetConfigMap();
        Task<string> GetConfigMapRoleMap();
        Task<string> ReplaceConfigMapRoleMap(string configMapRoleMap);
        Task<string> PatchConfigMapRoleMap(string configMapRoleMap);
    }
}