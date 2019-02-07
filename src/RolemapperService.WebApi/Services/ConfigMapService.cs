
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace RolemapperService.WebApi.Services
{
    public class ConfigMapService : IConfigMapService
    {
        public string AddReadOnlyRoleMapping(string configMapYaml, string roleName, string roleArn)
        {
            var groups = GetReadonlyGroup();
            var updatedMapRolesYaml = AddRoleMapping(configMapYaml, roleArn, roleName, groups);

            return updatedMapRolesYaml;
        }

        public string AddRoleMapping(
            string configMapYaml, 
            string roleArn, 
            string userName, 
            IList<string> groups
        )
        {
            // If the role map already exist return existing map.
            if (configMapYaml.Contains(roleArn))
            {
                return configMapYaml;
            }

            // Construct the new role-mapping.
            YamlMappingNode mapping = new YamlMappingNode();
            mapping.Add("rolearn", roleArn);
            mapping.Add("username", $"{userName}:{{{{SessionName}}}}");

            var groupsNodes = groups.Select(group => new YamlScalarNode(group));
            mapping.Add("groups", new YamlSequenceNode(groupsNodes));

            // The mapping is part of a sequence of role maps.
            var yamlSequenceNode = new YamlSequenceNode(mapping);
            // Create yaml-stream from the mapping, in order to save to string.
            var stream = new YamlStream(new YamlDocument(yamlSequenceNode));

            // Append the new role-mapping to the original configMapYaml. 
            var modifiedYaml = string.Empty;
            StringBuilder yamlStringBuilder = new StringBuilder();

            using (var writer = new StringWriter(yamlStringBuilder))
            {
                stream.Save(writer, false);
                modifiedYaml = configMapYaml + yamlStringBuilder.ToString().Replace("\n...", "");
            }

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