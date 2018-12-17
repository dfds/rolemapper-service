
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace RolemapperService.WebApi.Services
{
    public class ConfigMapService : IConfigMapService
    {
        public string AddRoleMapping(string yaml, string roleArn, string userName, IList<string> groups)
        {
            var modifiedYaml = yaml;

            using (StringReader reader = new StringReader(yaml))
            {
                var stream = new YamlStream();
                stream.Load(reader);

                // Get the root node "mapRoles".
                var rootNode = (YamlMappingNode)stream.Documents[0].RootNode;
                var mapRolesNode = (YamlSequenceNode)rootNode["mapRoles"];

                // Construct the new role-mapping and add it to mapRoles sequence.
                YamlMappingNode mapping = new YamlMappingNode();
                mapping.Add("roleARN", roleArn);
                mapping.Add("username", userName);

                var groupsNodes = groups.Select(group => new YamlScalarNode(group));
                mapping.Add("groups", new YamlSequenceNode(groupsNodes));

                mapRolesNode.Add(mapping);

                // Save the stream to string and return this.
                StringBuilder yamlStringBuilder = new StringBuilder();
                stream.Save(new StringWriter(yamlStringBuilder));

                modifiedYaml = yamlStringBuilder.ToString();
            }

            return modifiedYaml;
        }
    }
}