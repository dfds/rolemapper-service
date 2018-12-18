using System.Collections.Generic;

namespace RolemapperService.WebApi.Services
{
    public class KubernetesService : IKubernetesService
    {
        private static readonly string KubeSystemNamespace = "kube-system";
        private static readonly string AwsAuthConfigMapName = "aws-auth";

        public string GetAwsAuthConfigMap()
        {
            return GetConfigMap(KubeSystemNamespace, AwsAuthConfigMapName);
        }

        public string GetConfigMap(string namespaceName, string configMapName)
        {
            // TODO: Implement integration.
            return _mapRolesInput;
        }

        public bool PatchAwsAuthConfigMap(string configMapYaml)
        {
            return PatchConfigMap(KubeSystemNamespace, AwsAuthConfigMapName, configMapYaml);
        }

        public bool PatchConfigMap(string namespaceName, string configMapName, string configMapYaml)
        {
            // TODO: Implement integration.
            return true;
        }

        public bool ReplaceAwsAuthConfigMap(string configMapYaml)
        {
            return ReplaceConfigMap(KubeSystemNamespace, AwsAuthConfigMapName, configMapYaml);
        }

        public bool ReplaceConfigMap(string namespaceName, string configMapName, string configMapYaml)
        {
            // TODO: Implement integration.
            return true;
        }

        private readonly string _mapRolesInput = 
@"mapRoles:
- roleARN: arn:aws:iam::228426479489:role/KubernetesAdmin
  username: kubernetes-admin:{{SessionName}}
  groups:
  - system:masters
- roleARN: arn:aws:iam::228426479489:role/KubernetesView
  username: kubernetes-view:{{SessionName}}
  groups:
  - kub-view
";
    }
}