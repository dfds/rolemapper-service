using System;
using System.Collections.Generic;
using System.Text;

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

            var roleArnObjectText = CreateRoleArnObjectText(roleArn, userName, groups);

            if (string.IsNullOrWhiteSpace(configMapYaml))
            {
                return roleArnObjectText;
            }

            if (configMapYaml.EndsWith("\n") == false)
            {
                configMapYaml = configMapYaml + "\r\n";
            }
            configMapYaml = configMapYaml + roleArnObjectText;

            return configMapYaml;
        }

      public static string CreateRoleArnObjectText(
            string roleArn,
            string userName,
            IEnumerable<string> groups
        )
        {
            var builder = new StringBuilder();
            builder.AppendLine($"- rolearn: {roleArn}");
            builder.AppendLine($"  username: {userName}:{{{{SessionName}}}}");
            builder.AppendLine("  groups:");
            foreach (var @group in groups)
            {
                builder.AppendLine($"    - {@group}");
            }

            return builder.ToString();       
        }
    }
}