using k8s.Models;
using RolemapperService.WebApi.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RolemapperService.WebApi.Extensions
{
    public static class KubernetesExtensions
    {
        public static KubernetesConfigMap GetCustomConfigMap(this V1ConfigMap apiConfigMap)
        {
            return new KubernetesConfigMap
            {
                ApiVersion = apiConfigMap?.ApiVersion,
                Data = apiConfigMap?.Data,
                Kind = apiConfigMap?.Kind,
                Metadata = new Metadata
                {
                    Name = apiConfigMap?.Metadata?.Name,
                    NamespaceProperty = apiConfigMap?.Metadata?.NamespaceProperty
                }
            };
        }

        public static string SerializeToYaml(this V1ConfigMap apiConfigMap)
        {
            var customConfigMap = GetCustomConfigMap(apiConfigMap);

            var serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
                
            var yamlSerializedObject = serializer.Serialize(customConfigMap);

            return yamlSerializedObject;
        }
    }
}