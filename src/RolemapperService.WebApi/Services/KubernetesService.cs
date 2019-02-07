using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;

namespace RolemapperService.WebApi.Services
{
    public class KubernetesService : IKubernetesService
    {
        private readonly IKubernetesRepository _kubernetesRepository;
        private readonly IConfigMapService _configMapService;

        public KubernetesService(
            IKubernetesRepository kubernetesRepository, 
            IConfigMapService configMapService
        )
        {
            _kubernetesRepository = kubernetesRepository;
            _configMapService = configMapService;
        }


        public async Task<string> GetAwsAuthConfigMap()
        {
            return await _kubernetesRepository.GetAwsAuthConfigMap();
        }

        public async Task<string> PatchAwsAuthConfigMapRoleMap(
            string roleName, 
            string roleArn
        )
        {
            var configMapRoleMap = await _kubernetesRepository.GetAwsAuthConfigMapRoleMap();

            configMapRoleMap = _configMapService.AddReadOnlyRoleMapping(configMapRoleMap, roleName, roleArn);
            var patchedConfigMapRoleMap = await _kubernetesRepository.PatchAwsAuthConfigMapRoleMap(configMapRoleMap);

            return patchedConfigMapRoleMap;
        }

        public async Task<string> ReplaceAwsAuthConfigMapRoleMap(string roleName, string roleArn)
        {
            var configMapRoleMap = await _kubernetesRepository.GetAwsAuthConfigMapRoleMap();

            configMapRoleMap = _configMapService.AddReadOnlyRoleMapping(
                configMapRoleMap, 
                roleName, 
                roleArn
            );
            
            var newConfigMapRoleMap = 
                await _kubernetesRepository.ReplaceAwsAuthConfigMapRoleMap(configMapRoleMap);

            return newConfigMapRoleMap;
        }
    }
}