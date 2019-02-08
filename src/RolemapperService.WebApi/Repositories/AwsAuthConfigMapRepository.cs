using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Rest;
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

            awsAuthConfigMap = awsAuthConfigMap ?? new V1ConfigMap();
            var awsAuthConfigMapYaml = awsAuthConfigMap.SerializeToYaml();

            return awsAuthConfigMapYaml;
        }

        public async Task WriteConfigMapRoleMap(string configMapRoleMap)
        {
            var awsAuthConfigMap = await ReadConfigMap();
            if (awsAuthConfigMap == null)
            {
                awsAuthConfigMap = new V1ConfigMap
                {
                    Data = new Dictionary<string, string> {{"mapRoles", configMapRoleMap}},
                    Metadata = new V1ObjectMeta
                    {
                        Name = ConfigMapName,
                        NamespaceProperty = "kube-system"
                    }
                };
                
                
                await _client.CreateNamespacedConfigMapAsync(
                    body: awsAuthConfigMap,
                    namespaceParameter: ConfigMapNamespace
                );
                
                return;
            }

            awsAuthConfigMap.Data = new Dictionary<string, string>
            {
                {"mapRoles", configMapRoleMap}
            };

            await _client.ReplaceNamespacedConfigMapAsync(
                body: awsAuthConfigMap,
                name: ConfigMapName, 
                namespaceParameter: ConfigMapNamespace
            );
        }


        private async Task<V1ConfigMap> ReadConfigMap()
        {
            try
            {
                return await _client.ReadNamespacedConfigMapAsync(
                    name: ConfigMapName,
                    namespaceParameter: ConfigMapNamespace,
                    exact: true,
                    export: true
                );
            }
            catch (HttpOperationException httpOperationException) when (httpOperationException.Response.StatusCode ==
                                                                        HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}