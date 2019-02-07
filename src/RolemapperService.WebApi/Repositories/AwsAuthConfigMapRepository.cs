using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using RolemapperService.WebApi.Extensions;

namespace RolemapperService.WebApi.Repositories
{
    public class AwsAuthConfigMapRepository : IAwsAuthConfigMapRepository
    {
        private readonly IKubernetes _client;
        private static readonly string ConfigMapName = "aws-auth";
        private static readonly string ConfigMapNamespace = "kube-system";

        public AwsAuthConfigMapRepository(IKubernetes client)
        {
            _client = client;
        }

        public async Task<string> GetConfigMap()
        {
            var awsAuthConfigMap = await ReadConfigMap();
            var awsAuthConfigMapYaml = awsAuthConfigMap.SerializeToYaml();

            return awsAuthConfigMapYaml;
        }

        public async Task<string> GetConfigMapRoleMap()
        {
            var awsAuthConfigMap = await ReadConfigMap();
            var awsAuthConfigMapMapRoles = awsAuthConfigMap.Data["mapRoles"];

            return awsAuthConfigMapMapRoles;
        }

        public async Task<string> ReplaceConfigMapRoleMap(string configMapRoleMap)
        {
            var awsAuthConfigMap = await ReadConfigMap();

            awsAuthConfigMap.Data = new Dictionary<string, string>
            {
                { "mapRoles", configMapRoleMap }
            };

            var awsAuthConfigMapReplaced = await _client.ReplaceNamespacedConfigMapAsync(body: awsAuthConfigMap, name: ConfigMapName, namespaceParameter: ConfigMapNamespace);
            var awsAuthConfigMapYaml = awsAuthConfigMapReplaced.SerializeToYaml();

            return awsAuthConfigMapYaml;
        }

        public async Task<string> PatchConfigMapRoleMap(string configMapRoleMap)
        {
            var patch = new JsonPatchDocument<V1ConfigMap>();
            patch.Replace(c => c.Data["mapRoles"], configMapRoleMap);

            var configMapPatch = new V1Patch(patch);

            var awsAuthConfigMap = await _client.PatchNamespacedConfigMapAsync(body: configMapPatch, name: ConfigMapName, namespaceParameter: ConfigMapNamespace);
            var awsAuthConfigMapYaml = awsAuthConfigMap.SerializeToYaml();

            return awsAuthConfigMapYaml;
        }

        private async Task<V1ConfigMap> ReadConfigMap()
        {
            return await _client.ReadNamespacedConfigMapAsync(
                name: ConfigMapName, 
                namespaceParameter: ConfigMapNamespace, 
                exact: true, 
                export: true
            );
        }
    }
}