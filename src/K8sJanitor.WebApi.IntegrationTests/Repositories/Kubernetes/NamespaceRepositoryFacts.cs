using System;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using Xunit;

namespace K8sJanitor.WebApi.IntegrationTests.Repositories.Kubernetes
{
    public class NamespaceRepositoryFacts
    {
        [FactRunsOnK8s]
        public async Task A_Created_Namespace_Will_Have_the_KIAM_Annotation()
        {
            // Arrange
            var namespaceName = "namespace-from-test";
            var awsRoleName = "awsRoleName";
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new k8s.Kubernetes(config);

            var sut = new NamespaceRepository(client);

            try
            {
                // act
                await sut.CreateNamespace(namespaceName, awsRoleName);


                // Assert
                var namespaces = await client.ListNamespaceAsync();
                var @namespace = namespaces.Items.Single(n => n.Metadata.Name == namespaceName);


                var annotationValue = @namespace.Metadata.Annotations["iam.amazonaws.com/permitted"];

                Assert.Equal(awsRoleName, annotationValue);
            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: namespaceName
                );
            }
        }

        [Fact]
        public async Task Two_Namespace_Will_Create_A_Error()
        {
            // Arrange
            var namespaceName = "namespace-from-test";
            var awsRoleName = "awsRoleName";
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new k8s.Kubernetes(config);

            var sut = new NamespaceRepository(client);

            try
            {
                await sut.CreateNamespace(namespaceName, awsRoleName);
                
                
                // Act
                var ex = await Assert.ThrowsAsync<Exception>(async () => await sut.CreateNamespace(namespaceName, awsRoleName));

                // Assert
                var expectedStart = "Error occured while communicating with k8s:";
                Assert.StartsWith(expectedStart, ex.Message);
            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: namespaceName
                );
            }
        }
    }
}