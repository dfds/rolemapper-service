using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace RolemapperService.WebApi
{
    public class ConfigMapEditor
    {

        public static string AddRoleMapping(
            string configMapYaml,
            string roleArn,
            string userName,
            IEnumerable<string> groups
        )
        {
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
    }
}