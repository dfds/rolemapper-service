using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;

namespace RolemapperService.WebApi.Services
{
    public class KubernetesService : IKubernetesService
    {
        private readonly IAwsAuthConfigMapRepository _awsAuthConfigMapRepository;
        private readonly IConfigMapService _configMapService;

        public KubernetesService(
            IAwsAuthConfigMapRepository awsAuthConfigMapRepository, 
            IConfigMapService configMapService
        )
        {
            _awsAuthConfigMapRepository = awsAuthConfigMapRepository;
            _configMapService = configMapService;
        }


        public async Task<string> GetAwsAuthConfigMap()
        {
            return await _awsAuthConfigMapRepository.GetConfigMap();
        }

        public async Task<string> PatchAwsAuthConfigMapRoleMap(
            string roleName, 
            string roleArn
        )
        {
            var configMapRoleMap = await _awsAuthConfigMapRepository.GetConfigMapRoleMap();

            configMapRoleMap = _configMapService.AddReadOnlyRoleMapping(configMapRoleMap, roleName, roleArn);
            var patchedConfigMapRoleMap = await _awsAuthConfigMapRepository.PatchConfigMapRoleMap(configMapRoleMap);

            return patchedConfigMapRoleMap;
        }

      
    }
}