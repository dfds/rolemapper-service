using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;
using RolemapperService.WebApi.Services;

namespace RolemapperService.WebApi.Tests.TestDoubles
{
    public class KubernetesRepositoryStub : IKubernetesRepository
    {
        private readonly string _configMap = @"
        ApiVersion: v1
        Data:
        mapRoles: >
            - rolearn: arn:aws:iam::738063116313:role/eks-lovelace-node
            username: system:node:{{EC2PrivateDNSName}}
            groups:
                - system:bootstrappers
                - system:nodes
            - rolearn: arn:aws:iam::738063116313:role/ADFS-ViewOnly
            username: ADFS-ViewOnly:{{SessionName}}
            groups:
            - DFDS-ReadOnly
        Kind: ConfigMap
        Metadata:
        Annotations:
            kubectl.kubernetes.io/last-applied-configuration: >
            {""apiVersion"":""v1"",""data"":{""mapRoles"":""- rolearn: arn:aws:iam::738063116313:role/eks-lovelace-node\n  username: system:node:{{EC2PrivateDNSName}}\n  groups:\n    - system:bootstrappers\n    - system:nodes\n""},""kind"":""ConfigMap"",""metadata"":{""annotations"":{},""name"":""aws-auth"",""namespace"":""kube-system""}}
        CreationTimestamp: 2018-12-13T20:07:14.0000000Z
        Name: aws-auth
        NamespaceProperty: kube-system
        ResourceVersion: 3145552
        SelfLink: /api/v1/namespaces/kube-system/configmaps/aws-auth
        Uid: af3531c4-ff12-11e8-9421-02d6755a1e70";

        private readonly string _configMapRoleMap = @"
        mapRoles: >
            - rolearn: arn:aws:iam::738063116313:role/eks-lovelace-node
            username: system:node:{{EC2PrivateDNSName}}
            groups:
                - system:bootstrappers
                - system:nodes
            - rolearn: arn:aws:iam::738063116313:role/ADFS-ViewOnly
            username: ADFS-ViewOnly:{{SessionName}}
            groups:
            - DFDS-ReadOnly";

        public Task<string> GetAwsAuthConfigMap()
        {
            return Task.FromResult(_configMap);
        }

        public Task<string> GetAwsAuthConfigMapRoleMap()
        {
            return Task.FromResult(_configMapRoleMap);
        }

        public Task<string> PatchAwsAuthConfigMapRoleMap(string configMapRoleMap)
        {
            return Task.FromResult(configMapRoleMap);
        }

        public Task<string> ReplaceAwsAuthConfigMapRoleMap(string configMapRoleMap)
        {
            return Task.FromResult(configMapRoleMap);
        }
    }
}
