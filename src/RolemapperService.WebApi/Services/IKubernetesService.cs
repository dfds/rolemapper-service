using System.Collections.Generic;
using System.Threading.Tasks;
using k8s.Models;

namespace RolemapperService.WebApi.Services
{
    public interface IKubernetesService
    {
        Task<string> GetAwsAuthConfigMap();
        Task<string> PatchAwsAuthConfigMapRoleMap(string roleName, string roleArn);
        Task<string> ReplaceAwsAuthConfigMapRoleMap(string roleName, string roleArn);
    
    }
}