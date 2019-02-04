using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using RolemapperService.WebApi.Extensions;
using RolemapperService.WebApi.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RolemapperService.WebApi.Repositories
{
    public class KubernetesRepository : IKubernetesRepository
    {
        private readonly IKubernetes _client;
        private static readonly string ConfigMapName = "aws-auth";
        private static readonly string ConfigMapNamespace = "kube-system";

        public KubernetesRepository(IKubernetes client)
        {
            _client = client;
        }

        public async Task<string> GetAwsAuthConfigMap()
        {
            var awsAuthConfigMap = await GetConfigMap();
            var awsAuthConfigMapYaml = awsAuthConfigMap.SerializeToYaml();

            return awsAuthConfigMapYaml;
        }

        public async Task<string> GetAwsAuthConfigMapRoleMap()
        {
            var awsAuthConfigMap = await GetConfigMap();
            var awsAuthConfigMapMapRoles = awsAuthConfigMap.Data["mapRoles"];

            return awsAuthConfigMapMapRoles;
        }

        public async Task<string> ReplaceAwsAuthConfigMapRoleMap(string configMapRoleMap)
        {
            var awsAuthConfigMap = await GetConfigMap();

            awsAuthConfigMap.Data = new Dictionary<string, string>
            {
                { "mapRoles", configMapRoleMap }
            };

            var awsAuthConfigMapReplaced = await _client.ReplaceNamespacedConfigMapAsync(body: awsAuthConfigMap, name: ConfigMapName, namespaceParameter: ConfigMapNamespace);
            var awsAuthConfigMapYaml = awsAuthConfigMapReplaced.SerializeToYaml();

            return awsAuthConfigMapYaml;
        }

        public async Task<string> PatchAwsAuthConfigMapRoleMap(string configMapRoleMap)
        {
            var patch = new JsonPatchDocument<V1ConfigMap>();
            patch.Replace(c => c.Data["mapRoles"], configMapRoleMap);

            var configMapPatch = new V1Patch(patch);

            var awsAuthConfigMap = await _client.PatchNamespacedConfigMapAsync(body: configMapPatch, name: ConfigMapName, namespaceParameter: ConfigMapNamespace);
            var awsAuthConfigMapYaml = awsAuthConfigMap.SerializeToYaml();

            return awsAuthConfigMapYaml;
        }

        private async Task<V1ConfigMap> GetConfigMap()
        {
            return await _client.ReadNamespacedConfigMapAsync(name: ConfigMapName, namespaceParameter: ConfigMapNamespace, exact: true, export: true);
        }

        public async Task CreateNamespace(string namespaceName)
        {
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = namespaceName
                }
            };
            
            var result = await _client.CreateNamespaceAsync(ns);
        }

        public async Task<string> CreateNamespaceFullAccessClusterRole(string namespaceName)
        {
            var clusterRole = new V1ClusterRole
            {
                Metadata = new V1ObjectMeta
                {
                    Name = $"{namespaceName}-fullaccess",
                    NamespaceProperty = namespaceName
                },
                Rules = new List<V1PolicyRule>
                {
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "",
                            "extensions",
                            "apps"
                        },
                        Resources = new List<string>
                        {
                            "*"
                        },
                        // ResourceNames = new List<string>
                        // {
                        //     "*"
                        // },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "batch"
                        },
                        Resources = new List<string>
                        {
                            "jobs",
                            "cronjobs"
                        },
                        // ResourceNames = new List<string>
                        // {
                        //     "*"
                        // },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    }
                }
            };
            
            var result = await _client.CreateClusterRoleAsync(clusterRole);

            return result?.Metadata?.Name;
        }

        public async Task CreateGroupForRole(string groupName, string roleName)
        {
            // TODO: Group name should be something like "namespace-fullaccess".
            var clusterRoleBinding = new V1ClusterRoleBinding
            {
                Metadata = new V1ObjectMeta
                {
                    Name = $"{roleName}-to-{groupName}"
                },
                Subjects = new List<V1Subject>
                {
                    new V1Subject
                    {
                        Kind = "Group",
                        Name = groupName,
                        ApiGroup = "rbac.authorization.k8s.io"
                    }
                },
                RoleRef = new V1RoleRef
                {
                    Kind = "ClusterRole",
                    Name = roleName,
                    ApiGroup = "rbac.authorization.k8s.io"
                }
            };
            
            var result = await _client.CreateClusterRoleBindingAsync(clusterRoleBinding);
        }
    }
}