using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;

namespace RolemapperService.WebApi.Services
{
    public class RoleService : IRoleService
    {
        private readonly IConfigMapPersistanceService _configMapPersistanceService;
        private readonly IAwsAuthConfigMapRepository _awsAuthConfigMapRepository;
        private readonly IConfigMapService _configMapService;

        public RoleService(
            IConfigMapPersistanceService configMapPersistanceService,
            IAwsAuthConfigMapRepository awsAuthConfigMapRepository,
            IConfigMapService configMapService
        )
        {
            _configMapPersistanceService = configMapPersistanceService;
            _awsAuthConfigMapRepository = awsAuthConfigMapRepository;
            _configMapService = configMapService;
        }

        public async Task AddRole(
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
            var groups = new[] {"DFDS-ReadOnly", roleName};
            var configMapRoleMap = await _configMapService.AddRoleMapping(
                roleName,
                roleArn,
                groups
            );

            var newConfigMapRoleMap =
                await _awsAuthConfigMapRepository.ReplaceConfigMapRoleMap(configMapRoleMap);

            return newConfigMapRoleMap;
        }
    }
}