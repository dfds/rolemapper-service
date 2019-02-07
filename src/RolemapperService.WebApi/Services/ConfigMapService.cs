
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

        public async Task<string> AddReadOnlyRoleMapping(string roleName, string roleArn)
        {
            var groups = GetReadonlyGroup();
            var updatedMapRolesYaml = AddRoleMapping(roleArn, roleName, groups);

            return await updatedMapRolesYaml;
        }

        public async Task<string> AddRoleMapping(
            string roleArn, 
            string userName, 
            IList<string> groups
        )
        {
            
        var configMapYaml = await _awsAuthConfigMapRepository.GetConfigMap();
            // If the role map already exist return existing map.
            if (configMapYaml.Contains(roleArn))
            {
                return configMapYaml;
            }

            var modifiedYaml = ConfigMapEditor.AddRoleMapping(
                configMapYaml,
                roleArn,
                userName,
                groups
            );

            return modifiedYaml;
        }

        private IList<string> GetReadonlyGroup()
        {
            // TODO: Get from configuration?
            return new List<string>
            {
                "DFDS-ReadOnly"
            };
        }
    }
}