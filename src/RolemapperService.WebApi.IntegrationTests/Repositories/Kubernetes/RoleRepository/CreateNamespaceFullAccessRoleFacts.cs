using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using RolemapperService.WebApi.Repositories.Kubernetes;
using RolemapperService.WebApi.Wrappers;
using Xunit;

namespace RolemapperService.WebApi.IntegrationTests.Repositories.Kubernetes.RoleRepository
{
    // Reads as RoleRepository.CreateNamespaceFullAccessRole can
    public class CreateNamespaceFullAccessRoleFacts
    {
        [FactRunsOnK8s]
        public async Task Create_A_Role_For_A_Existing_Namespace()
        {
            // Arrange
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

            var client = new k8s.Kubernetes(config);
            var wrapper = new KubernetesWrapper(client);

            var namespaceRepository = new NamespaceRespoitory(client);
            var subjectNameSpace = "namespace-with-role-test";
            var awsRoleName = "notUSed";

            var sut = new WebApi.Repositories.Kubernetes.RoleRepository(wrapper);
            try
            {
                // Act
                await namespaceRepository.CreateNamespace(subjectNameSpace, awsRoleName);
                await sut.CreateNamespaceFullAccessRole(subjectNameSpace);

                // Assert
                client.ListNamespacedRole(subjectNameSpace).Items.Single();
            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: subjectNameSpace
                );
            }
        }
    }
}