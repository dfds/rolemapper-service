using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

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

        public async Task<string> GetAwsAuthConfigMapRoleMap()
        {
            var awsAuthConfigMap = await _client.ReadNamespacedConfigMapAsync(name: ConfigMapName, namespaceParameter: ConfigMapNamespace);
            var awsAuthConfigMapMapRoles = awsAuthConfigMap.Data["mapRoles"];

            return awsAuthConfigMapMapRoles;
        }

        public async Task<string> ReplaceAwsAuthConfigMapRoleMap(string configMapRoleMap)
        {
            // TODO: When ready to test actually replacing the config map in Kubernetes - remove line below.
            return await Task.FromResult<string>(configMapRoleMap);
            
            var configMap = new V1ConfigMap
            {
                Data = new Dictionary<string, string>
                {
                    { "mapRoles", configMapRoleMap }
                }
            };

            var awsAuthConfigMap = await _client.ReplaceNamespacedConfigMapAsync(body: configMap, name: ConfigMapName, namespaceParameter: ConfigMapNamespace);
            var awsAuthConfigMapMapRoles = awsAuthConfigMap.Data["mapRoles"];

            return awsAuthConfigMapMapRoles;
        }

        public async Task<string> PatchAwsAuthConfigMapRoleMap(string configMapRoleMap)
        {
            // TODO: Investigate how to use the patch object.
            var configMapPatch = new V1Patch();

            var awsAuthConfigMap = await _client.PatchNamespacedConfigMapAsync(body: configMapPatch, name: ConfigMapName, namespaceParameter: ConfigMapNamespace);
            var awsAuthConfigMapMapRoles = awsAuthConfigMap.Data["mapRoles"];

            return awsAuthConfigMapMapRoles;
        }
    }
}