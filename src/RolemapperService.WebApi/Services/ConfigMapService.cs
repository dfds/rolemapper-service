
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RolemapperService.WebApi.Extensions;
using RolemapperService.WebApi.Repositories;
using YamlDotNet.RepresentationModel;

namespace RolemapperService.WebApi.Services
{
    public class ConfigMapService : IConfigMapService
    {
        private readonly IAwsAuthConfigMapRepository _awsAuthConfigMapRepository;
        private readonly IConfigMapPersistanceService _configMapPersistenceService;

        public ConfigMapService(
            IAwsAuthConfigMapRepository awsAuthConfigMapRepository, 
            IConfigMapPersistanceService configMapPersistenceService
        )
        {
            _awsAuthConfigMapRepository = awsAuthConfigMapRepository;
            _configMapPersistenceService = configMapPersistenceService;
        }


        public async Task AddRole(
            string roleName,
            string roleArn
        )
        {
            var configMap = await _awsAuthConfigMapRepository.GetConfigMap();

            configMap.Data = configMap.Data ?? new Dictionary<string, string> {{"mapRoles", ""}};
            var groups = new[] {"DFDS-ReadOnly", roleName};
            var modifiedYaml = ConfigMapEditor.AddRoleMapping(
                configMap.Data["mapRoles"],
                roleArn,
                roleName,
                groups
            );
            configMap.Data["mapRoles"] = modifiedYaml;
            
            await _awsAuthConfigMapRepository.WriteConfigMap(configMap);
            
            var awsAuthConfigMapYaml = configMap.SerializeToYaml();
            await _configMapPersistenceService.StoreConfigMap(awsAuthConfigMapYaml);
        }
    }
}