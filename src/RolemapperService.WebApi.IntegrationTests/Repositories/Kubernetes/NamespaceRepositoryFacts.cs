using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using RolemapperService.WebApi.Repositories.Kubernetes;
using Xunit;

namespace RolemapperService.WebApi.IntegrationTests.Repositories.Kubernetes
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

            var sut = new NamespaceRespoitory(client);

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
    }
}