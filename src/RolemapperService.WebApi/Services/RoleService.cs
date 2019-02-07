using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;

namespace RolemapperService.WebApi.Services
{
    public class RoleService : IRoleService
    {
        private readonly IConfigMapPersistanceService _configMapPersistanceService;
        private readonly IKubernetesRepository _kubernetesRepository;
        private readonly IConfigMapService _configMapService;

        public RoleService(
            IConfigMapPersistanceService configMapPersistanceService,
            IKubernetesRepository kubernetesRepository,
            IConfigMapService configMapService
        )
        {
            _configMapPersistanceService = configMapPersistanceService;
            _kubernetesRepository = kubernetesRepository;
            _configMapService = configMapService;
        }

        public async Task CreateRole(
            string roleName,
            string roleArn
        )
        {
            var mapRolesYaml = await ReplaceAwsAuthConfigMapRoleMap(
                roleName,
                roleArn
            );
            await _configMapPersistanceService.StoreConfigMap(mapRolesYaml);
        }


        public async Task<string> ReplaceAwsAuthConfigMapRoleMap(string roleName, string roleArn)
        {
            var configMapRoleMap = await _kubernetesRepository.GetAwsAuthConfigMapRoleMap();


            var groups = new[] {"DFDS-ReadOnly", roleName};
            configMapRoleMap = _configMapService.AddRoleMapping(
                configMapRoleMap,
                roleName,
                roleArn,
                groups
            );

            var newConfigMapRoleMap =
                await _kubernetesRepository.ReplaceAwsAuthConfigMapRoleMap(configMapRoleMap);

            return newConfigMapRoleMap;
        }
    }
}