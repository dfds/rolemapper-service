using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace RolemapperService.WebApi.Models
{
    public class KubernetesConfigMap
    {
        public string ApiVersion { get; set; }
        public IDictionary<string, string> Data { get; set; }
        public string Kind { get; set; }
        public Metadata Metadata { get; set; }
    }

    public class Metadata
    {
        public string Name { get; set; }
        [YamlMember(Alias = "namespace")]
        public string NamespaceProperty { get; set; }
    }
}