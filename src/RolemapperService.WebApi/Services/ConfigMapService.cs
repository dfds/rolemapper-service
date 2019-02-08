
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;
using YamlDotNet.RepresentationModel;

namespace RolemapperService.WebApi.Services
{
    public class ConfigMapService : IConfigMapService
    {
        private readonly IAwsAuthConfigMapRepository _awsAuthConfigMapRepository;
        private readonly IConfigMapPersistanceService _configMapPersistenceService;


        public async Task AddRole(
            string roleName,
            string roleArn
        )
        {
            var configMapYaml = await _awsAuthConfigMapRepository.GetConfigMap();

            var groups = new[] {"DFDS-ReadOnly", roleName};
            var modifiedYaml = ConfigMapEditor.AddRoleMapping(
                configMapYaml,
                roleArn,
                roleName,
                groups
            );
            
            
            await _awsAuthConfigMapRepository.ReplaceConfigMapRoleMap(modifiedYaml);
            await _configMapPersistenceService.StoreConfigMap(modifiedYaml);
        }
    }
}