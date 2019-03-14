using Xunit;
using K8sJanitor.WebApi.Validators;
using K8sJanitor.WebApi.Models;
using k8s.Models;
using K8sJanitor.WebApi.Extensions;
using System.Collections.Generic;

namespace K8sJanitor.WebApi.Tests
{
    public class KubernetesExtensionsTest
    {
        [Fact]
        public void GetCustomConfigMap_GivenValidInput_MapsToCustomObject()
        {
            // Arrange
            var kubeConfigMap = GetConfigMap();

            // Act
            var customConfigMap = kubeConfigMap.GetCustomConfigMap();

            // Assert
            Assert.NotNull(customConfigMap);
            Assert.Equal(kubeConfigMap.ApiVersion, customConfigMap.ApiVersion);
            Assert.Equal(kubeConfigMap.Data["mapRoles"], customConfigMap.Data["mapRoles"]);
            Assert.Equal(kubeConfigMap.Kind, customConfigMap.Kind);
            Assert.Equal(kubeConfigMap.Metadata.Name, customConfigMap.Metadata.Name);
            Assert.Equal(kubeConfigMap.Metadata.NamespaceProperty, customConfigMap.Metadata.NamespaceProperty);
        }

        [Fact]
        public void SerializeToYaml_GivenValidInput_SerializeExpectedFormat()
        {
            // Arrange
            var kubeConfigMap = GetConfigMap();

            // Act
            var configMapYaml = kubeConfigMap.SerializeToYaml();

            // Assert
            Assert.NotEmpty(configMapYaml);
            // Camel case properties
            Assert.Contains("apiVersion", configMapYaml);
            Assert.Contains("metadata", configMapYaml);
            // "Namespace" and not "NamespaceProperty"
            Assert.Contains("namespace", configMapYaml);
        }

        private V1ConfigMap GetConfigMap()
        {
            return new V1ConfigMap
            {
                ApiVersion = "v1",
                Data = new Dictionary<string, string>
                {
                    { 
                        "mapRoles",
                         @">
                            - rolearn: arn:aws:iam::738063116313:role/eks-lovelace-node
                            username: system:node:{{EC2PrivateDNSName}}
                            groups:
                                - system:bootstrappers
                                - system:nodes
                            - rolearn: arn:aws:iam::738063116313:role/OneManArmy
                            username: OneManArmy:{{SessionName}}
                            groups:
                            - DFDS-ReadOnly
                            - rolearn: arn:aws:iam::738063116313:role/TeamOJ
                            username: TeamOJ:{{SessionName}}
                            groups:
                            - DFDS-ReadOnly"
                    }
                },
                Kind = "ConfigMap",
                Metadata = new V1ObjectMeta
                {
                    Name = "aws-auth",
                    NamespaceProperty = "kube-system"
                }
            };
        }
    }
}
