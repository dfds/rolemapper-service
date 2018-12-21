using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;

namespace RolemapperService.WebApi.Services
{
    public class KubernetesService : IKubernetesService
    {
        private readonly IKubernetesRepository _kubernetesRepository;
        private readonly IConfigMapService _configMapService;

        public KubernetesService(IKubernetesRepository kubernetesRepository, IConfigMapService configMapService)
        {
            _kubernetesRepository = kubernetesRepository;
            _configMapService = configMapService;
        }

        public async Task<string> GetAwsAuthConfigMapRoleMap()
        {
            return await _kubernetesRepository.GetAwsAuthConfigMapRoleMap();
        }

        public async Task<string> PatchAwsAuthConfigMapRoleMap(string roleName, string roleArn)
        {
            // TODO: Determine what is needed for the patch object in KubernetesRepository.
            var configMapRoleMap = string.Empty;
            var patchedConfigMapRoleMap = await _kubernetesRepository.PatchAwsAuthConfigMapRoleMap(configMapRoleMap);

            return patchedConfigMapRoleMap;
        }

        public async Task<string> ReplaceAwsAuthConfigMapRoleMap(string roleName, string roleArn)
        {
            var configMapRoleMap = await _kubernetesRepository.GetAwsAuthConfigMapRoleMap();

            configMapRoleMap = _configMapService.AddReadOnlyRoleMapping(configMapRoleMap, roleName, roleArn);
            var newConfigMapRoleMap = await _kubernetesRepository.ReplaceAwsAuthConfigMapRoleMap(configMapRoleMap);

            return newConfigMapRoleMap;
        }
    }
}