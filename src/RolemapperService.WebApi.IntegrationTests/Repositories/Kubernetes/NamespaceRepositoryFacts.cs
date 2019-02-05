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
        public async Task Can_Create_A_Namespace()
        {
            // Arrange
            var namespaceName = "namespace-from-test";
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new k8s.Kubernetes(config);

            var sut = new NamespaceRespoitory(client);

            try
            {
                // act
                await sut.CreateNamespace(namespaceName);

                
                // Assert
                var namespaces = await client.ListNamespaceAsync();
                var namespacesNames = namespaces.Items.Select(n => n.Metadata.Name);
                
                Assert.Contains(namespaceName, namespacesNames);
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